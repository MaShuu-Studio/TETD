using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapMoreInfo : MonoBehaviour
{
    [SerializeField] private Image elementImage;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI speedText;
    public void SetInfo(MapProperty prop)
    {
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)prop.element);

        string str = "-";
        if (prop.atk > 0) str = $"+{Mathf.Abs(prop.atk)}%";
        else if (prop.atk < 0) str = $"-{Mathf.Abs(prop.atk)}%";
        atkText.text = str;

        str = "-";
        if (prop.atkSpeed > 0) str = $"+{Mathf.Abs(prop.atkSpeed)}%";
        else if (prop.atkSpeed < 0) str = $"-{Mathf.Abs(prop.atkSpeed)}%";
        atkSpeedText.text = str;

        str = "-";
        if (prop.hp > 0) str = $"+{Mathf.Abs(prop.hp)}%";
        else if (prop.hp < 0) str = $"-{Mathf.Abs(prop.hp)}%";
        hpText.text = str;

        str = "-";
        if (prop.speed > 0) str = $"+{Mathf.Abs(prop.speed)}%";
        else if (prop.speed < 0) str = $"-{Mathf.Abs(prop.speed)}%";
        speedText.text = str;
    }
}
