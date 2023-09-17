using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfoPanel : TowerInfo
{
    [Space]
    [SerializeField] private Image gradeImage;

    [Space]
    [SerializeField] private AttackPriorityToggle priorityTogglePrefab;
    [SerializeField] private Transform priorityParent;
    private List<AttackPriorityToggle> priorityToggles;

    [SerializeField] private TowerUpgradeItem[] upgradeItems;
    [SerializeField] private TextMeshProUGUI valueText;

    private RectTransform rectTransform;
    public RectTransform RectTransform { get { return rectTransform; } }

    private TowerObject selectedTower;


    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        priorityToggles = new List<AttackPriorityToggle>();
        for (int i = 0; i < EnumArray.AttackPrioritys.Length; i++)
        {
            int id = (int)SpriteManager.ETCDataNumber.APRIORITY + i;
            AttackPriorityToggle toggle = Instantiate(priorityTogglePrefab, priorityParent);
            toggle.gameObject.SetActive(true);
            toggle.SetIcon(id, this, i);
            priorityToggles.Add(toggle);
        }
        priorityTogglePrefab.gameObject.SetActive(false);
    }

    public override void SetData(Tower data)
    {
        base.SetData(data);

        gradeImage.color = data.GradeColor;

        selectedTower = TowerController.Instance.SelectedTower;
        valueText.text = "$ " + data.Value();
        int index = (int)selectedTower.Priority;
        for (int p = 0; p < priorityToggles.Count; p++)
            priorityToggles[p].isOn = p == index;

        priorityToggles[(int)AttackPriority.DEBUFF].gameObject.SetActive(data.HasDebuff);
        int i = 0;
        for (; i < EnumArray.TowerStatTypes.Length; i++)
        {
            TowerStatType type = EnumArray.TowerStatTypes[i];
            upgradeItems[i].gameObject.SetActive(true);
            UpdateUpgradeStat(i, type);
        }

        for (; i < upgradeItems.Length; i++)
        {
            upgradeItems[i].gameObject.SetActive(false);
        }
        UpdateInfo();
        //UpdateBonusStat();
    }
    /*
    public void UpdateBonusStat()
    {
        for (int i = 0; i < bonusStatObjects.Length; i++)
        {
            float bonus = 0;
            if (selectedTower != null) bonus = selectedTower.BonusStat((TowerStatType)i);
            if (bonus == 0) bonusStatObjects[i].SetActive(false);
            else
            {
                bonusStatObjects[i].SetActive(true);
                bonusStatTexts[i].text = string.Format("{0:0.#}", bonus);
            }
        }
    }
    */


    public override void UpdateInfo()
    {
        if (selectedTower == null) return;

        for (int i = 0; i < mainStats.Length; i++)
        {
            TowerStatType type = (TowerStatType)i;
            mainStats[i].SetData(selectedTower.Stat(type));
        }

        if (data.AbilityTypes != null && data.AbilityTypes.Count > 0)
        {
            for (int i = 0; i < data.AbilityTypes.Count; i++)
            {
                AbilityType type = data.AbilityTypes[i];
                abilities[i].SetData(data.Ability(type));
            }
        }
    }

    public void Reinforce(int index, TowerStatType type)
    {
        if (PlayerController.Instance.Buy(selectedTower.Data.UpgradeCost(type)))
        {
            selectedTower.Data.Upgrade(type);
            selectedTower.UpdateDistnace();
            valueText.text = "$ " + data.Value();

            UpdateUpgradeStat(index, type);
            UpdateInfo();
            //UpdateBonusStat();
        }
    }

    public void UpdateUpgradeStat(int index, TowerStatType type)
    {
        int level = selectedTower.Data.StatLevel(type);
        int cost = PlayerController.Cost(selectedTower.Data.UpgradeCost(type));

        upgradeItems[index].SetData(index, type, level, cost);
    }

    // 토글 작동시 ValueChanged를 통해 조정
    public void SetPriority(int index)
    {
        if (selectedTower == null) return;

        selectedTower.ChangePriority((AttackPriority)index);
    }
}
