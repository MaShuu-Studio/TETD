using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(Image))]
public class AttackPriorityToggle : MonoBehaviour
{
    public bool isOn
    {
        get
        {
            if (toggle == null) toggle = GetComponent<Toggle>();
            return toggle.isOn;
        }

        set
        {
            toggle.isOn = value;
            ChangeColor(value);
        }
    }

    [SerializeField] private Image mask;
    private DescriptionIcon desc;
    private Toggle toggle;
    private Image image;

    private static Color g = new Color(0.25f, 0.25f, 0.25f, 1);
    private int index;

    public void SetIcon(int id, TowerInfoPanel info, int index)
    {
        image = GetComponent<Image>();
        toggle = GetComponent<Toggle>();
        desc = GetComponent<DescriptionIcon>();

        Sprite sprite = SpriteManager.GetSprite(id);
        image.sprite = mask.sprite = sprite;
        image.color = Color.white;
        mask.color = new Color(0, 0, 0, 0.7f);
        desc.SetIcon(id);
        this.index = index;
        toggle.onValueChanged.AddListener(b => info.SetPriority(this.index));
        toggle.onValueChanged.AddListener(b => ChangeColor(b));
    }
    private void ChangeColor(bool b)
    {
        if (b) image.color = Color.white;
        else image.color = g;
    }
}
