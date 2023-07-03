using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

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
    public string backgroundName;
    public Vector3Int origin;
    public Vector3Int size;
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfo(string backgroundName)
    {
        this.backgroundName = backgroundName;
        this.origin = Vector3Int.zero;
        this.size = Vector3Int.zero;
        this.tiles = new Dictionary<Vector3Int, TileInfo>();
    }

    public TilemapInfo(TilemapInfoJson data)
    {
        this.backgroundName = data.backgroundName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);
    }

    public TilemapInfo(TilemapInfo data)
    {
        this.backgroundName = data.backgroundName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);
    }

    public CustomRuleTile GetTile(Vector3Int pos)
    {
        if (tiles.ContainsKey(pos) == false) return null;
        return TileManager.GetTile(tiles[pos]);
    }

    public Sprite[] GetBackGround()
    {
        return TileManager.GetBackground(backgroundName);
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
    public string type;
    public string name;

    public CustomTile Base
    {
        get
        {
            if (tiles == null) return null;
            return tiles[0];
        }
    }

    public CustomRuleTile(string name, string type, CustomTile[] tiles)
    {
        this.tiles = tiles;
        this.name = name;
        this.type = type;
    }

    public CustomTile GetTile(string[] info)
    {
        // 상 하 좌 우
        ushort index = 0;
        for (int i = 0; i < 4; i++)
        {
            index = (ushort)(index << 1);

            if (type == info[i]) index += 1;
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
    public string backgroundName;
    public Vector3Int origin;
    public Vector3Int size;
    public List<Vector3Int> tileKeys = new List<Vector3Int>();
    public List<TileInfo> tileValues = new List<TileInfo>();
    public Dictionary<Vector3Int, TileInfo> tiles;

    public TilemapInfoJson(TilemapInfo info)
    {
        this.backgroundName = info.backgroundName;
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