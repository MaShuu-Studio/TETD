using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ScriptableButton : MonoBehaviour
{
    protected Button button;
    protected void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ClickEvent());
    }
    protected abstract void ClickEvent();
}
