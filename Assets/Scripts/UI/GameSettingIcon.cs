using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(Image))]
public class GameSettingIcon : MonoBehaviour
{
    protected Toggle toggle;
    protected Image image;
    private static Color g = new Color(0.25f, 0.25f, 0.25f, 1);

    public bool isOn
    {
        get { return toggle.isOn; }
        set
        {
            if (toggle == null) toggle = GetComponent<Toggle>();
            if (image == null) image = GetComponent<Image>();
            toggle.isOn = value;
        }
    }

    public void SetIcon(GameSettingController gameSetting, Sprite sprite, string str)
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (image == null) image = GetComponent<Image>();
        image.sprite = sprite;
        toggle.onValueChanged.AddListener(b => ChangeColor(b));
        toggle.onValueChanged.AddListener(b => gameSetting.ShowInfo(b, str));
        toggle.isOn = false;
        ChangeColor(false);
    }

    public void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = g;
    }
}
