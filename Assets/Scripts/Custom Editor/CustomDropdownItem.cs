using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class CustomDropdownItem : MonoBehaviour
{
    private CustomDropdown target;
    private int index;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(b => ClickEvent(b));
    }

    public void SetOption(CustomDropdown target, int index, CustomDropdownOption option)
    {
        this.target = target;
        this.index = index;

        if (itemImage != null) itemImage.sprite = option.sprite;
        if (itemText != null) itemText.text = option.str;
    }

    private void ClickEvent(bool b)
    {
        if (b) target.Select(index);
    }
}
