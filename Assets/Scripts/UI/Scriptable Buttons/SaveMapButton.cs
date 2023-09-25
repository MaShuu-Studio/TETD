using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMapButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        if (MapEditor.Instance.CanSave)
        {
            Map map = MapEditor.Instance.MapData;
            CustomDataManager.SaveMap(MapEditor.Instance.MapName, map);
        }
    }
}
