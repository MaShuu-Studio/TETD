using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Data;

[CustomEditor(typeof(MapController))]
public class MapEditor : Editor
{
    private MapController targetComponent;
    private const string path = "Assets/Resources/Map/";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool generate = GUILayout.Button("Generate", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        bool load = GUILayout.Button("Load", GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (generate)
        {
            DataManager.Serialize<Map>(path, targetComponent.map);
        }

        if (load)
        {            
            targetComponent.map = DataManager.Deserialize<Map>(path);
        }
    }

    private void OnSceneGUI()
    {
        targetComponent = (MapController)target;

        List<Vector2> roads = targetComponent.map.enemyRoad;
        
        Vector2 prevPos = Vector2.zero; 
        for (int i = 0; i < roads.Count; i++)
        { 
            /*if (i != 0)
            {
                float xamount = Mathf.Abs(roads[i].x - prevPos.x);
                float yamount = Mathf.Abs(roads[i].y - prevPos.y);

                // 적의 이동 방향은 일직선상이어야 함. 
                if (xamount != 0 && yamount != 0)
                {
                    continue;
                }
            }*/
            roads[i] = Handles.PositionHandle(roads[i], Quaternion.identity);
            roads[i] = targetComponent.GetTilePos(roads[i]);
            prevPos = roads[i];
        }

        Vector2 cellsize = targetComponent.grid.cellSize;
        for (int i = 0; i < roads.Count; i++)
        {
            Color c = Color.magenta;
            if (i == 0 || i == roads.Count - 1)
                c = Color.red;

            c.a = 0.5f;

            Handles.DrawSolidRectangleWithOutline(new Rect(roads[i] - cellsize / 2, cellsize), c, c);

            if (i != 0) 
                Handles.DrawPolyLine(roads[i - 1], roads[i]);
        }
    }
}
