using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerStatItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Init(TowerStatType type)
    {
        icon.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.TOWERSTAT,(int)type);
    }
    public void Init(BuffType type)
    {
        icon.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.BUFF,(int)type);
    }
    public void Init(DebuffType type)
    {
        icon.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.DEBUFF,(int)type);
    }
    public void SetData(float value)
    {
        valueText.text = string.Format("{0:0.#}", value);
    }
}
