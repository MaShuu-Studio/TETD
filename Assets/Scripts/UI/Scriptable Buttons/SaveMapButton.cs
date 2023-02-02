using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMapButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        if (MapEditor.Instance.CanSave)
        {
            TilemapInfo info = MapEditor.Instance.Tilemap;
            MapManager.SaveMap(MapEditor.Instance.MapName, info);
            GameController.Instance.Title();
        }
    }
}
