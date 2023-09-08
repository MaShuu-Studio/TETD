using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class SelectCustomDataButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Toggle toggle;
    private Button button;

    private int index;

    public void Init(string name, CustomEditor editor, int index)
    {
        button = GetComponent<Button>();
        toggle.isOn = false;
        text.text = name;
        this.index = index;
        button.onClick.AddListener(() => editor.SelectData(this.index));
        toggle.onValueChanged.AddListener(b => editor.AddData(this.index, b));
    }
}
