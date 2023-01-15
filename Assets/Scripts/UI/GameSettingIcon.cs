using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(Image))]
public class GameSettingIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI desc;
    private Toggle toggle;
    private Image image;
    public bool isOn
    {
        get { return toggle.isOn; }
        set
        {
            toggle.isOn = value;
            ChangeColor(value);
        }
    }

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();
    }
    public void SetIcon(string str)
    {
        desc.text = str;
    }

    public void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = Color.gray * 0.5f;
    }
}
