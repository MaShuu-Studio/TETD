using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfoPanel : TowerInfo
{
    [Space]
    [SerializeField] private Toggle[] priorityToggles;

    [SerializeField] private GameObject[] bonusStatObjects;
    [SerializeField] private TextMeshProUGUI[] bonusStatTexts;
    [SerializeField] private TowerReinforceButton[] reinforceButtons;
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

        selectedTower = TowerController.Instance.SelectedTower;
        valueText.text = "$ " + data.Value();
        int index = (int)selectedTower.Priority;
        priorityToggles[index].isOn = true;

        priorityToggles[(int)AttackPriority.DEBUFF].gameObject.SetActive(data.hasDebuff);

        for (int i = 0; i < 3; i++)
        {
            UpdateUpgradeStat((TowerStatType)i);
        }

        UpdateBonusStat();
    }

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

    public void Reinforce(TowerStatType type)
    {
        if (PlayerController.Instance.Buy(selectedTower.Data.UpgradeCost(type)))
        {
            selectedTower.Data.Upgrade(type);
            selectedTower.UpdateDistnace();
            valueText.text = "$ " + data.Value();

            UpdateUpgradeStat(type);
            UpdateInfo();
            UpdateBonusStat();
        }
    }

    public void UpdateUpgradeStat(TowerStatType type)
    {
        int level = selectedTower.Data.StatLevel(type);
        int cost = PlayerController.Cost(selectedTower.Data.UpgradeCost(type));
        float cur = selectedTower.Data.Stat(type);
        float next = cur * 1.1f;

        reinforceButtons[(int)type].SetData(level, cost, cur, next);
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
