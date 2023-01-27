using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIndRouteButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        TilemapInfo tilemap = MapEditor.Instance.MakeMap();
        MapEditor.Instance.FindRoute(tilemap);
    }
}
