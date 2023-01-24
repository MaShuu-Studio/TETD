using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumData;

public class TowerReinforceButton : ScriptableButton
{
    [SerializeField] private TowerStatType type;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI curText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI upgradeText;

    protected override void ClickEvent()
    {
        UIController.Instance.ReinforceTower(type);
    }

    public void SetData(int level, int cost, float cur, float next)
    {
        levelText.text = string.Format("Lv.{0}", level);
        curText.text = string.Format("{0:0.#}", cur);
        nextText.text = string.Format("{0:0.#}", next);

        if (level == 5)
        {
            button.interactable = false;
            upgradeText.text = "MAX";
        }
        else
        {
            upgradeText.text = string.Format("$ {0}", cost);
            button.interactable = true;
        }
    }
}
