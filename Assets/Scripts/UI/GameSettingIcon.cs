using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(Image))]
public class GameSettingIcon : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI desc;
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
            ChangeColor(value);
        }
    }

    protected void Awake()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (image == null) image = GetComponent<Image>();
    }
    public void SetIcon(string str)
    {
        desc.text = str;
    }

    public virtual void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = Color.gray * 0.5f;
    }
}
