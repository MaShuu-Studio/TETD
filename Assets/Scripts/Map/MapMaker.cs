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
        List<Vector3Int> road = MapUtil.FindRoute(tilemap);

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
                    bool b = (tn != "ROAD" && tn != "START" && tn != "DEST");
                    buildableTilemap.SetTile(pos, (b) ? buildableTile : notBuildableTile);
                }
            }
        }
    }

    public TilemapInfo MakeMap()
    {
        mapTilemap.origin = buildableTilemap.origin = routeTilemap.origin = new Vector3Int(size.x / -2, size.y / -2);
        mapTilemap.size = buildableTilemap.size = routeTilemap.size = new Vector3Int(size.x, size.y, 1);

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
                    bool b = (tn != "ROAD" && tn != "START" && tn != "DEST");
                    mapInfo.Add(pos, new TileInfo(tile, b));
                }
            }
        }

        TilemapInfo tilemap = new TilemapInfo(mapTilemap.origin, mapTilemap.size, mapInfo);

        if (FindRoute(tilemap) == false)
        {
            Debug.Log("[SYSTEM] CAN NOT MAKE MAP");
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
            TileInfo tile = tilemapInfo.tiles[pos];
            mapTilemap.SetTile(pos, tile.tile);
        }
        FindRoute(tilemapInfo);
    }
}
