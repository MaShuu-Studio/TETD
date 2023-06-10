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
    public string tileName;
    public Vector3Int origin;
    public Vector3Int size;
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfo(string tileName)
    {
        this.tileName = tileName;
        this.origin = Vector3Int.zero;
        this.size = Vector3Int.zero;
        this.tiles = new Dictionary<Vector3Int, TileInfo>();
    }

    public TilemapInfo(TilemapInfoJson data)
    {
        this.tileName = data.tileName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);
    }

    public TilemapInfo(string tileName, Vector3Int origin, Vector3Int size, Dictionary<Vector3Int, TileInfo> tiles)
    {
        this.tileName = tileName;
        this.origin = origin;
        this.size = size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(tiles);
    }

    public TilemapInfo(TilemapInfo data)
    {
        this.tileName = data.tileName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);
    }

    public CustomRuleTile GetTile(Vector3Int pos)
    {
        if (tiles.ContainsKey(pos) == false) return null;
        return TileManager.GetTile(tileName, tiles[pos]);
    }

    public Sprite[] GetBackGround()
    {
        return TileManager.GetBackground(tileName);
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

public class TilePalette
{
    public string tileName;
    public Sprite[] backgrounds;
    public Dictionary<string, CustomRuleTile> buildable;
    public Dictionary<string, CustomRuleTile> notBuildable;
    public Dictionary<string, CustomRuleTile> roads;
    public List<CustomRuleTile> Tiles { get; private set; }

    public TilePalette(string tileName, 
        Dictionary<string, CustomRuleTile> buildable, 
        Dictionary<string, CustomRuleTile> notBuildable, 
        Dictionary<string, CustomRuleTile> roads,
        Sprite[] backgrounds)
    {
        this.tileName = tileName;
        this.buildable = buildable;
        this.notBuildable = notBuildable;
        this.roads = roads;

        Tiles = new List<CustomRuleTile>();
        foreach (var tile in buildable.Values)
            Tiles.Add(tile);
        foreach (var tile in notBuildable.Values)
            Tiles.Add(tile);
        foreach (var tile in roads.Values)
            Tiles.Add(tile);

        this.backgrounds = new Sprite[backgrounds.Length];
        backgrounds.CopyTo(this.backgrounds, 0);
    }
}

public class CustomRuleTile
{
    // 1111 : 상 하 좌 우
    // 비트 연산 활용
    // 0000 : □
    // 0001 : ⊂
    // 0010 : ⊃
    // 0011 : =
    // 0100 : ∩
    // 0101 : ┌
    // 0110 : ┐
    // 0111 : ─(위)
    // 1000 : ∪
    // 1001 : └
    // 1010 : ┘
    // 1011 : ─(아래)
    // 1100 : ||
    // 1101 :│  (좌)
    // 1110 :  │(우)
    // 1111 : X
    // □⊂⊃=
    // ∩┌┐─
    // ∪└┘─
    // ||││X

    private CustomTile[] tiles;
    public string name;

    public CustomTile Base
    {
        get
        {
            if (tiles == null) return null;
            return tiles[0];
        }
    }

    public CustomRuleTile(string name, CustomTile[] tiles)
    {
        this.tiles = tiles;
        this.name = name;
    }

    public CustomTile GetTile(string[] info)
    {
        // 상 하 좌 우
        ushort index = 0;
        for (int i = 0; i < 4; i++)
        {
            index = (ushort)(index << 1);

            if ((name == info[i]) ||
                (TileManager.IsConstTile(name) && TileManager.IsConstTile(info[i])))
                index += 1;
        }

        return tiles[index];
    }
}

public class CustomTile : Tile
{
    public bool buildable;

    public void SetData(string name, Sprite sprite, bool buildable)
    {
        this.name = name;
        this.sprite = sprite;
        this.buildable = buildable;
    }
}

// 실 사용할 데이터와 직렬화할 데이터를 구분
[Serializable]
public class TilemapInfoJson : ISerializationCallbackReceiver
{
    public string tileName;
    public Vector3Int origin;
    public Vector3Int size;
    public List<Vector3Int> tileKeys = new List<Vector3Int>();
    public List<TileInfo> tileValues = new List<TileInfo>();
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfoJson(TilemapInfo info)
    {
        this.tileName = info.tileName;
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