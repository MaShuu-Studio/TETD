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
    [SerializeField] private Toggle[] priorityToggles;

    [SerializeField] private TowerUpgradeItem[] upgradeItems;
    [SerializeField] private TextMeshProUGUI valueText;

    private RectTransform rectTransform;
    public RectTransform RectTransform { get { return rectTransform; } }

    private TowerObject selectedTower;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void SetData(Tower data)
    {
        base.SetData(data);

        gradeImage.color = data.GradeColor;

        selectedTower = TowerController.Instance.SelectedTower;
        valueText.text = "$ " + data.Value();
        int index = (int)selectedTower.Priority;
        priorityToggles[index].isOn = true;

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

        if (data.AbilityTypes.Length > 0)
        {
            for (int i = 0; i < data.AbilityTypes.Length; i++)
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
    public void SetPriority(bool b)
    {
        // 하나만 작동하도록 false면 return
        if (b == false) return;

        for (int i = 0; i < priorityToggles.Length; i++)
        {
            if (priorityToggles[i].isOn)
            {
                selectedTower.ChangePriority((AttackPriority)i);
                break;
            }
        }
    }
}
