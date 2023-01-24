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
        string name = $"TowerStatType{(int)type}";
        icon.sprite = SpriteManager.GetSprite(name);
    }
    public void SetData(float value)
    {
        valueText.text = string.Format("{0:0.#}", value);
    }
}
