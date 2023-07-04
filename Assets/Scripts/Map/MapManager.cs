using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Data;
using System;
using System.Threading.Tasks;

public static class MapManager
{
    private static string path = Application.streamingAssetsPath + "/Data/Map/";

    public static List<string> Maps { get { return maps; } }
    private static List<string> maps;
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static async Task GetTotal()
    {
        string[] files = await DataManager.GetFiles(path);
        TotalProgress = files.Length;
    }

    public static async Task Init()
    {
        maps = new List<string>();
        string[] files = await DataManager.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            CurProgress++;

            maps.Add(DataManager.FileNameTriming(files[i]));
        }

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD MAP {maps.Count}");
#endif
    }
    /*
    public static Tuple<bool, List<Vector3Int>> FindRoute(TilemapInfo tilemap)
    {
        if (tilemap == null) return null;

        bool finds = false, findd = false;
        Vector3Int start = Vector3Int.zero;
        Vector3Int dest = Vector3Int.zero;

        for (int y = tilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < tilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + tilemap.origin;
                CustomRuleTile tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    if (tn == "START")
                    {
                        start = new Vector3Int(pos.x, pos.y);
                        finds = true;
                    }
                    if (tn == "DEST")
                    {
                        dest = new Vector3Int(pos.x, pos.y);
                        findd = true;
                    }
                    if (finds && findd) break;
                }
            }
        }

        List<Vector3Int> road = new List<Vector3Int>();
        Vector3Int[] dirArray = { Vector3Int.down, Vector3Int.right, Vector3Int.up, Vector3Int.left };
        int dirIndex = 0;
        Vector3Int dir = dirArray[dirIndex];
        road.Add(start);
        int max = tilemap.size.x * tilemap.size.y;

        bool findRoute = false;

        Vector3Int curPos = road[0];
        for (int i = 0; i < max; i++)
        {
            Vector3Int nextPos = curPos + dir;

            CustomRuleTile nextTile = tilemap.GetTile(nextPos);

            // 다음 스텝이 목적지라면 끝
            if (nextPos == dest)
            {
                road.Add(dest);
                findRoute = true;
                break;
            }

            // 가는 방향의 다음 스텝이 길일 경우
            if (nextTile != null && nextTile.name.ToUpper() == "ROAD")
            {
                curPos = nextPos;
            }
            else
            {
                // 아니라면 다른 방향을 탐색함.
                // 해당 과정에서 왔던 길과 탐색했던 길은 제외함.
                // 단, 첫 출발일 경우에는 왔던 길이 없으므로 반대 방향을 제외하지 않음.
                Vector3Int origin = dir;
                List<Vector3Int> newDir = new List<Vector3Int>(dirArray);
                newDir.Remove(dir);
                if (i != 0) newDir.Remove(dir * -1);
                for (int j = 0; j < newDir.Count; j++)
                {
                    nextPos = curPos + newDir[j];

                    nextTile = tilemap.GetTile(nextPos);
                    if (nextTile != null && (nextTile.name.ToUpper() == "ROAD" || nextTile.name.ToUpper() == "DEST"))
                    {
                        dir = newDir[j];
                        break;
                    }
                }
                // 모든 길이 막혀있다면 길이 없음.
                if (dir == origin) break;
                else if (road[road.Count - 1] != curPos)
                    road.Add(curPos);
            }
        }

        findRoute = finds && findd && road.Count > 1 && road[road.Count - 1] == dest;

        return new Tuple<bool, List<Vector3Int>>(findRoute, road);
    }
    */
    public static void SaveMap(string mapName, TilemapInfo info)
    {
        TilemapInfoJson data = new TilemapInfoJson(info);
        DataManager.SerializeJson(path, mapName, data);
        if (maps.Contains(mapName) == false) maps.Add(mapName);
    }

    public static async Task<Map> LoadMap(string mapName)
    {
        TilemapInfoJson data = await DataManager.DeserializeJson<TilemapInfoJson>(path, mapName);
        if (data == null) return null;

        TilemapInfo info = new TilemapInfo(data);

        return new Map(mapName, info);
    }
}
