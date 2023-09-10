using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LibraryFilterToggle : Toggle
{
    private Image img;
    public void Init(int type, int num, bool b = true)
    {
        img = GetComponent<Image>();
        img.sprite = SpriteManager.GetSprite(type + num);
        isOn = b;
        if (isOn) img.color = Color.white;
        else img.color = Color.gray * 0.5f;

        onValueChanged.AddListener(isOn => Event(isOn));
    }

    private void Event(bool isOn)
    {
        UIController.Instance.UpdateLibrary();
        if (isOn) img.color = Color.white;
        else img.color = Color.gray * 0.5f;
    }
}
