using System.Collections;
using System.Collections.Generic;
using EnumData;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerAbilityItem : MonoBehaviour
{
    [SerializeField] private DescriptionIcon icon;
    [SerializeField] private GameObject[] abilityIcons;
    [SerializeField] private TextMeshProUGUI[] abilityTexts;
    public void Init(AbilityType type)
    {
        icon.SetIcon((int)SpriteManager.ETCDataNumber.TOWERABILITY + (int)type);
    }
    public void SetData(TowerAbility ability)
    {
        abilityIcons[0].SetActive(ability.value != 0);
        abilityIcons[1].SetActive(ability.atkSpeed != 0);
        abilityIcons[2].SetActive(ability.time != 0);

        abilityTexts[0].text = string.Format("{0:0.##}", ability.value);
        abilityTexts[1].text = string.Format("{0:0.##}", ability.atkSpeed);
        abilityTexts[2].text = string.Format("{0:0.##}", ability.time);
    }
}
