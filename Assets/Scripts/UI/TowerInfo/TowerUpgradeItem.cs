using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerUpgradeItem : MonoBehaviour
{
    [SerializeField] private DescriptionIcon icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    public int Id { get { return id; } }
    private int id;
    public int Index { get { return index; } }
    private int index;

    public void SetData(int index, int id, int level, int cost)
    {
        this.index = index;
        this.id = id;

        icon.SetIcon(this.id);

        SetInfo(level, cost);
    }

    private void SetInfo(int level, int cost)
    {
        levelText.text = string.Format("Lv.{0}", level);

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
