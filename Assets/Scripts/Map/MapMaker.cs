using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapMaker : MonoBehaviour
{
    private Grid grid;

    [SerializeField] private Vector2Int size;
    [SerializeField] private Tilemap mapTilemap;
    [SerializeField] private Tilemap buildableTilemap;
    [SerializeField] private Tilemap routeTilemap;
    [Space]
    [SerializeField] private TileBase buildableTile;
    [SerializeField] private TileBase notBuildableTile;
    [Space]
    [SerializeField] private TileBase startFlag;
    [SerializeField] private TileBase destFlag;
    [SerializeField] private TileBase cornorFlag;

    public static string Path { get; private set; } = "Assets/Resources/Map/";

    public void Clear()
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();
        routeTilemap.ClearAllTiles();
    }
    public void ClearInfo()
    {
        buildableTilemap.ClearAllTiles();
        routeTilemap.ClearAllTiles();
    }

    public List<Vector3Int> FindRoute()
    {
        UpdateMap();

        bool finds = false, findd = false;
        Vector3Int start = Vector3Int.zero;
        Vector3Int dest = Vector3Int.zero;
        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
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
        int max = mapTilemap.size.x * mapTilemap.size.y;

        bool findRoute = false;

        Vector3Int curPos = road[0];
        for (int i = 0; i < max; i++)
        {
            Vector3Int nextPos = curPos + dir;

            TileBase nextTile = mapTilemap.GetTile(nextPos);

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

                    nextTile = mapTilemap.GetTile(nextPos);
                    if (nextTile != null && nextTile.name.ToUpper() == "ROAD")
                    {
                        dir = newDir[j];
                        break;
                    }
                }
                // 모든 길이 막혀있다면 길이 없음.
                if (dir == origin) break;
                else
                {
                    road.Add(curPos);
                    curPos = curPos + dir;
                }
            }
        }

        if (findRoute == false)
        {
            road.Clear();
            road = null;
        }
        else
        {
            routeTilemap.ClearAllTiles();
            for (int i = 0; i < road.Count; i++)
            {
                if (i == 0) routeTilemap.SetTile(road[i], startFlag);
                else if (i == road.Count - 1) routeTilemap.SetTile(road[i], destFlag);
                else routeTilemap.SetTile(road[i], cornorFlag);
            }
        }

        return road;
    }

    public void UpdateMap()
    {
        buildableTilemap.ClearAllTiles();
        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    bool b = (tn != "ROAD" && tn != "START" && tn != "DEST");
                    buildableTilemap.SetTile(pos, (b) ? buildableTile : notBuildableTile);
                }
            }
        }
    }

    public TilemapInfo MakeMap(string mapName)
    {
        List<Vector3Int> route = FindRoute();
        if (route == null)
        {
            Debug.Log("[SYSTEM] CAN NOT MAKE MAP");
            return null;
        }

        mapTilemap.origin = buildableTilemap.origin = routeTilemap.origin = new Vector3Int(size.x / -2, size.y / -2);
        mapTilemap.size = buildableTilemap.size = routeTilemap.size = new Vector3Int(size.x, size.y, 1);

        UpdateMap();
        List<TileInfo> mapInfo = new List<TileInfo>();
        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    bool b = (tn != "ROAD" && tn != "START" && tn != "DEST");
                    mapInfo.Add(new TileInfo(pos, tile, b));
                }
            }
        }

        return new TilemapInfo(mapTilemap.origin, mapTilemap.size, mapInfo);
    }

    public void LoadMap(TilemapInfo tilemapInfo)
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();
        mapTilemap.origin = tilemapInfo.origin;
        mapTilemap.size = tilemapInfo.size;

        for (int i = 0; i < tilemapInfo.tiles.Count; i++)
        {
            TileInfo tile = tilemapInfo.tiles[i];
            mapTilemap.SetTile(tile.pos, tile.tile);
        }

        FindRoute();
    }
}
