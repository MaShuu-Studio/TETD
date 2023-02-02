using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTile : ScriptableButton
{
    [SerializeField] private Image tileImage;
    private CustomRuleTile data;

    public void SetTile(CustomRuleTile data)
    {
        this.data = data;
        tileImage.sprite = data.Base.sprite;
    }

    protected override void ClickEvent()
    {
        MapEditor.Instance.SelectTile(data);
    }
}
