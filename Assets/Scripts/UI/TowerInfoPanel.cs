using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : TowerInfo
{
    [Space]
    [SerializeField] private Toggle[] priorityToggles;

    [SerializeField] private GameObject[] bonusStatObjects;
    [SerializeField] private TextMeshProUGUI[] bonusStatTexts;
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
        int index = (int)selectedTower.Priority;
        priorityToggles[index].isOn = true;

        UpdateBonusStat();
    }

    public void UpdateBonusStat()
    {
        for (int i = 0; i < bonusStatObjects.Length; i++)
        {
            float bonus = 0;
            if (selectedTower != null) bonus = selectedTower.BonusStat((EnumData.TowerMainStatType)i);
            if (bonus == 0) bonusStatObjects[i].SetActive(false);
            else
            {
                bonusStatObjects[i].SetActive(true);
                bonusStatTexts[i].text = string.Format("{0:0.#}", bonus);
            }
        }
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
                selectedTower.ChangePriority((EnumData.AttackPriority)i);
                break;
            }
        }
    }
}
