using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitEditorAdvancedOption : MonoBehaviour
{
    [SerializeField] private TMP_InputField spfInput;
    [SerializeField] private TMP_InputField[] attackTimeInputs;
    [Space]
    [Header("Effect")]
    [SerializeField] private UnitEditorSetImageIcon effectImageIcon;
    [SerializeField] private TMP_InputField effectColorInput;
    [SerializeField] private Image effectColorIcon;
    [SerializeField] private TMP_InputField effectSpfInput;

    [Space]
    [Header("Type")]
    [SerializeField] private Toggle[] typeToggles;
    [Space]
    [Header("Projectile")]
    [SerializeField] private UnitEditorSetImageIcon projImageIcon;
    [SerializeField] private TMP_InputField projRemainTimeInput;
    [SerializeField] private TMP_InputField projSpfInput;
    [SerializeField] private TMP_InputField projAttackTimeInput;

    public void Init()
    {
        effectColorInput.onValueChanged.AddListener(s => Hexadecimal());
    }

    private void Hexadecimal()
    {
        string str = effectColorInput.text;
        string newStr = "";
        foreach (var c in str)
        {
            char ch = 'F';
            if (c <= 'a' && c >= 'z')
                ch = (char)(c - ('a' - 'A'));

            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))
                ch = c;

            newStr += ch;
        }
    }
}
