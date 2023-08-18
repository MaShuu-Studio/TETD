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

    public void SetIcon(GameSettingController gameSetting, string str)
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (image == null) image = GetComponent<Image>();
        toggle.onValueChanged.AddListener(b => ChangeColor(b));
        toggle.onValueChanged.AddListener(b => gameSetting.ShowInfo(b, str));
        toggle.isOn = false;
        ChangeColor(false);
    }

    public void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = Color.gray * 0.5f;
    }
}
