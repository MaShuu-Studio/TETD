using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Map
{
    public string name;
    public TilemapInfo tilemap;
    public List<Vector3Int> enemyRoad;

    public Map(string name, TilemapInfo info, List<Vector3Int> road)
    {
        this.name = name;
        tilemap = new TilemapInfo(info);
        enemyRoad = new List<Vector3Int>(road);
    }
}

public class TilemapInfo
{
    public Vector3Int origin;
    public Vector3Int size;
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfo(Vector3Int origin, Vector3Int size, Dictionary<Vector3Int, TileInfo> tiles)
    {
        this.origin = origin;
        this.size = size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(tiles);
    }

    public TilemapInfo(TilemapInfo info)
    {
        this.origin = info.origin;
        this.size = info.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(info.tiles);
    }

    public CustomTile GetTile(Vector3Int pos)
    {
        if (tiles.ContainsKey(pos) == false) return null;
        return TileManager.GetTile(tiles[pos].name);
    }

    public bool Buildable(Vector3Int pos)
    {
        if (tiles.ContainsKey(pos) == false) return false;
        return tiles[pos].buildable;
    }
}

[Serializable]
public struct TileInfo
{
    public string name;
    public bool buildable;

    public TileInfo(string name, bool b)
    {
        this.name = name;
        this.buildable = b;
    }
}

public class CustomTile : Tile
{
    public void SetData(string name, Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}

// 실 사용할 데이터와 직렬화할 데이터를 구분
[Serializable]
public class TilemapInfoJson : ISerializationCallbackReceiver
{
    public Vector3Int origin;
    public Vector3Int size;
    public List<Vector3Int> tileKeys = new List<Vector3Int>();
    public List<TileInfo> tileValues = new List<TileInfo>();
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfoJson(TilemapInfo info)
    {
        this.origin = info.origin;
        this.size = info.size;
        this.tiles = info.tiles;
    }

    public void OnBeforeSerialize()
    {
        tileKeys.Clear();
        tileValues.Clear();

        foreach (var kvp in tiles)
        {
            tileKeys.Add(kvp.Key);
            tileValues.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        tiles = new Dictionary<Vector3Int, TileInfo>();

        for (int i = 0; i < tileKeys.Count && i < tileValues.Count; i++)
        {
            tiles.Add(tileKeys[i], tileValues[i]);
        }
    }
}