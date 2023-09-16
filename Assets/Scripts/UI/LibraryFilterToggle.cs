using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LibraryFilterToggle : Toggle
{
    private Image img;
    private DescriptionIcon desc;
    public void Init(int type, int num, bool b = true)
    {
        img = GetComponent<Image>();
        desc = GetComponent<DescriptionIcon>();

        int id = type + num;
        img.sprite = SpriteManager.GetSprite(id);
        ((RectTransform)transform).sizeDelta = Vector2.one * img.sprite.texture.width * 3;
        desc.SetIcon(id);
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
