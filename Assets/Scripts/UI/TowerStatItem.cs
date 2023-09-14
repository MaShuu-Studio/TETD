using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerStatItem : MonoBehaviour
{
    [SerializeField] private DescriptionIcon icon;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Init(TowerStatType type)
    {
        icon.SetIcon((int)SpriteManager.ETCDataNumber.TOWERSTAT + (int)type);
    }
    public void Init(AbilityType type)
    {
        icon.SetIcon((int)SpriteManager.ETCDataNumber.TOWERABILITY + (int)type);
    }
    public void Init(EnemyStatType type)
    {
        icon.SetIcon((int)SpriteManager.ETCDataNumber.ENEMYSTAT + (int)type);
    }
    public void SetData(float value)
    {
        valueText.text = string.Format("{0:0.##}", value);
    }
}
