using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapMaker : MonoBehaviour
{
    private Grid grid;

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

    // 루트를 보여주는 용도
    public bool FindRoute(TilemapInfo tilemap)
    {
        List<Vector3Int> road = MapManager.FindRoute(tilemap);

        if (road == null) return false;

        routeTilemap.ClearAllTiles();
        UpdateMap();
        for (int i = 0; i < road.Count; i++)
        {
            if (i == 0) routeTilemap.SetTile(road[i], startFlag);
            else if (i == road.Count - 1) routeTilemap.SetTile(road[i], destFlag);
            else routeTilemap.SetTile(road[i], cornorFlag);
        }
        return true;
    }

    // 빌드 가능한 곳을 단순히 보여주는 용도
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
                    bool b = (tn != "WALL" && tn != "ROAD" && tn != "START" && tn != "DEST");
                    buildableTilemap.SetTile(pos, (b) ? buildableTile : notBuildableTile);
                }
            }
        }
    }

    public TilemapInfo MakeMap()
    {
        UpdateMap();

        Dictionary<Vector3Int, TileInfo> mapInfo = new Dictionary<Vector3Int, TileInfo>();
        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    bool b = (tn != "WALL" && tn != "ROAD" && tn != "START" && tn != "DEST");
                    mapInfo.Add(pos, new TileInfo(tile.name, b));
                }
            }
        }

        TilemapInfo tilemap = new TilemapInfo(mapTilemap.origin, mapTilemap.size, mapInfo);

        if (FindRoute(tilemap) == false)
        {
#if UNITY_EDITOR
            Debug.Log("[SYSTEM] CAN NOT MAKE MAP");
#endif
            return null;
        }
        return tilemap;
    }

    public void LoadMap(TilemapInfo tilemapInfo)
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();
        mapTilemap.origin = tilemapInfo.origin;
        mapTilemap.size = tilemapInfo.size;

        foreach (var pos in tilemapInfo.tiles.Keys)
        {
            TileInfo tileInfo = tilemapInfo.tiles[pos];
            TileBase tile = TileManager.GetTile(tileInfo.name);
            mapTilemap.SetTile(pos, tile);
        }
        FindRoute(tilemapInfo);
    }
}
