using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Data;

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
        if (find) route = targetComponent.FindRoute();

        if (generate)
        {
            TilemapInfo info = targetComponent.MakeMap(mapName);
            route = targetComponent.FindRoute();
            if (string.IsNullOrEmpty(mapName) == false && info != null)
            {
                DataManager.SerializeJson<TilemapInfo>(MapMaker.Path, mapName, info);
            }
            else if (string.IsNullOrEmpty(mapName)) Debug.Log("[SYSTEM] Input map name");
        }

        if (load)
        {
            if (string.IsNullOrEmpty(mapName) == false)
            {
                TilemapInfo info = DataManager.DeserializeJson<TilemapInfo>(MapMaker.Path, mapName);
                targetComponent.LoadMap(info);
                route = targetComponent.FindRoute();
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
