using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    public TowerStatType Type { get { return type; } }
    private TowerStatType type;
    public int Index { get { return index; } }
    private int index;

    public void SetData(int index, TowerStatType type, int level, int cost)
    {
        this.index = index;
        this.type = type;

        iconImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.TOWERSTAT,(int)type);

        SetInfo(level, cost);
    }
    /*
    public void SetData(int index, BuffType type, int level, int cost)
    {
        this.index = index;
        this.type = type;

        string name = $"BuffType{(int)type}";
        iconImage.sprite = SpriteManager.GetSprite(name);

        SetInfo(level, cost);
    }

    public void SetData(int index, DebuffType type, int level, int cost)
    {
        this.index = index;
        this.type = type;

        string name = $"DebuffType{(int)type}";
        iconImage.sprite = SpriteManager.GetSprite(name);

        SetInfo(level, cost);
    }
    */
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
