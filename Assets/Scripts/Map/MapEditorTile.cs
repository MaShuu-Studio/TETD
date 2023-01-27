using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTile : ScriptableButton
{
    [SerializeField] private Image tileImage;
    private CustomTile data;

    public void SetTile(CustomTile data)
    {
        this.data = data;
        tileImage.sprite = data.sprite;
    }

    protected override void ClickEvent()
    {
        MapEditor.Instance.SelectTile(data);
    }
}
