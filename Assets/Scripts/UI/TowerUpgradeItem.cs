using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerUpgradeItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI curText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    public TowerStatType Type { get { return type; } }
    private TowerStatType type;
    public int Index { get { return index; } }
    private int index;
    public void SetData(int index, TowerStatType type, int level, int cost, float cur, float next)
    {
        this.index = index;
        this.type = type;

        nameText.text = EnumArray.TowerStatTypeStrings[type];

        levelText.text = string.Format("Lv.{0}", level);
        curText.text = string.Format("{0:0.#}", cur);
        nextText.text = string.Format("{0:0.#}", next);

        if (level == 5)
        {
            button.interactable = false;
            costText.text = "MAX";
        }
        else
        {
            costText.text = string.Format("$ {0}", cost);
            button.interactable = true;
        }
    }
}
