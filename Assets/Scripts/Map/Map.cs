using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Map
{
    public TilemapInfo tilemap;
    public List<Vector3Int> enemyRoad;

    public Map(TilemapInfo info, List<Vector3Int> road)
    {
        tilemap = new TilemapInfo(info);
        enemyRoad = new List<Vector3Int>(road);
    }
}

[Serializable]
public class TilemapInfo
{
    public Vector3Int origin;
    public Vector3Int size;
    public List<TileInfo> tiles;

    public TilemapInfo(Vector3Int origin, Vector3Int size, List<TileInfo> tiles)
    {
        this.origin = origin;
        this.size = size;
        this.tiles = new List<TileInfo>(tiles);
    }

    public TilemapInfo(TilemapInfo info)
    {
        this.origin = info.origin;
        this.size = info.size;
        this.tiles = new List<TileInfo>(info.tiles);
    }
}

[Serializable]
public struct TileInfo
{
    public Vector3Int pos;
    public TileBase tile;
    public bool buildable;

    public TileInfo(Vector3Int pos, TileBase tile, bool b)
    {
        this.pos = pos;
        this.tile = tile;
        this.buildable = b;
    }
}
