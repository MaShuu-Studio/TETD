using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Data;

#if UNITY_EDITOR
[CustomEditor(typeof(MapMaker))]
public class MapMakerEditor : Editor
{
    private MapMaker targetComponent;
    private static List<Vector3Int> route = null;
    private static string mapName;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        targetComponent = (MapMaker)target;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool clear = GUILayout.Button("All Clear", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        bool clearInfo = GUILayout.Button("Clear Info", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool find = GUILayout.Button("Find Route", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        GUILayout.Label("MAP NAME", GUILayout.Width(80));
        mapName = GUILayout.TextField(mapName);
        GUILayout.Space(50);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool generate = GUILayout.Button("Generate", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        bool load = GUILayout.Button("Load", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (clear)
        {
            route = null;
            targetComponent.Clear();
        }
        if (clearInfo)
        {
            route = null;
            targetComponent.ClearInfo();
        }
        if (find)
        {
            TilemapInfo info = targetComponent.MakeMap();
            route = MapManager.FindRoute(info);
            targetComponent.FindRoute(info);
        }

        if (generate)
        {
            if (string.IsNullOrEmpty(mapName) == false)
            {
                TilemapInfo info = targetComponent.MakeMap();
                route = MapManager.FindRoute(info);
                MapManager.SaveMap(mapName, info);
            }
            else if (string.IsNullOrEmpty(mapName)) Debug.Log("[SYSTEM] Input map name");
        }

        if (load)
        {
            if (string.IsNullOrEmpty(mapName) == false)
            {
                Map map = MapManager.LoadMap(mapName);
                route = MapManager.FindRoute(map.tilemap);
                targetComponent.LoadMap(map.tilemap);
            }
            else Debug.Log("[SYSTEM] Input map name");
        }
    }

    private void OnSceneGUI()
    {
        targetComponent = (MapMaker)target;

        if (route != null)
        {
            for (int i = 1; i < route.Count; i++)
            {
                Handles.color = Color.magenta;
                Handles.DrawLine(route[i - 1] + new Vector3(0.5f, 0.5f), route[i] + new Vector3(0.5f, 0.5f), 2);
            }
        }
    }
}
#endif