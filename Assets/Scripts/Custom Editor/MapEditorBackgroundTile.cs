using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorBackgroundTile : ScriptableButton
{
    [SerializeField] private Image[] backgrounds;
    private string backgroundName;

    public void SetTile(string name, Sprite[] sprites)
    {
        backgroundName = name;
        backgrounds[0].gameObject.SetActive(false);
        backgrounds[1].gameObject.SetActive(false);

        if (sprites != null)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (sprites[i] == null) continue;
                backgrounds[i].gameObject.SetActive(true);
                backgrounds[i].sprite = sprites[i];
            }
        }
    }

    protected override void ClickEvent()
    {
        MapEditor.Instance.SetBackground(backgroundName);
    }
}
