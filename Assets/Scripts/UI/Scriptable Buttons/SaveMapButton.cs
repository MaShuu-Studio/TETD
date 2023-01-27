using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMapButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        TilemapInfo info = MapEditor.Instance.MakeMap();
        bool savable = MapEditor.Instance.FindRoute(info);
        if (savable)
        {
            MapManager.SaveMap(MapEditor.Instance.MapName, info);
            GameController.Instance.Title();
        }
    }
}
