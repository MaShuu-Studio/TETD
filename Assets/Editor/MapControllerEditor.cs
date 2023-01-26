using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    private MapController targetComponent;
    private string mapName;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        targetComponent = (MapController)target;

        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        GUILayout.Label("MAP NAME", GUILayout.Width(80));
        mapName = GUILayout.TextField(mapName);
        GUILayout.Space(20);
        bool load = GUILayout.Button("Load", GUILayout.Width(120));
        GUILayout.Space(50);
        GUILayout.EndHorizontal();

        if (load)
        {
            //Map map = MapManager.LoadMap(mapName);
            //targetComponent.Init(map);
        }
    }
}
#endif