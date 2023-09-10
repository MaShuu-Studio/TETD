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

    private GameSettingController gameSetting;
    private string infoName;
    private string desc;

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
    private void Update()
    {
        if (gameSetting != null && UIController.PointOverUI(gameObject))
        {
            gameSetting.ShowInfo(infoName, desc);
        }
    }

    public void SetIcon(GameSettingController gameSetting, Sprite sprite, Language lang)
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (image == null) image = GetComponent<Image>();
        image.sprite = sprite;
        infoName = lang.name;
        desc = lang.desc;
        this.gameSetting = gameSetting;

        toggle.onValueChanged.AddListener(b => ChangeColor(b));
        toggle.isOn = false;
        ChangeColor(false);
    }

    public void UpdateLanguage(Language lang)
    {
        infoName = lang.name;
        desc = lang.desc;
    }

    public void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = g;
    }
}
