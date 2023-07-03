using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTile : ScriptableButton
{
    [SerializeField] private Image tileImage;
    private CustomRuleTile data;
    private bool isFlag;

    public void SetTile(CustomRuleTile data, bool isFlag = false)
    {
        this.data = data;
        tileImage.sprite = data.Base.sprite;
        this.isFlag = isFlag;
    }

    protected override void ClickEvent()
    {
        MapEditor.Instance.SelectTile(data, isFlag);
    }
}
