using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using EnumData;

public class Map
{
    public string name;

    public string backgroundName;
    public Vector3Int origin;
    public Vector3Int size;
    public Dictionary<Vector3Int, TileInfo> tiles;
    public List<Vector3Int> enemyRoad;

    public List<MapProperty> mapProperties;
    public Dictionary<Vector3Int, TileProperty> tileProperties;

    public List<Round> rounds;

    public Map()
    {
        name = "";

        backgroundName = "";
        origin = Vector3Int.zero;
        size = Vector3Int.zero;
        tiles = new Dictionary<Vector3Int, TileInfo>();
        enemyRoad = new List<Vector3Int>();
        mapProperties = new List<MapProperty>();
        tileProperties = new Dictionary<Vector3Int, TileProperty>();
        rounds = new List<Round>();
    }

    public Map(string name, MapDataJson data)
    {
        this.name = name;

        this.backgroundName = data.backgroundName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);

        if (data.enemyRoad != null) enemyRoad = data.enemyRoad;
        else enemyRoad = new List<Vector3Int>();

        mapProperties = data.mapProperties;
        tileProperties = data.tileProperties;

        rounds = data.rounds;
    }

    public Map(string name, Map data)
    {
        this.name = name;

        this.backgroundName = data.backgroundName;
        this.origin = data.origin;
        this.size = data.size;
        this.tiles = new Dictionary<Vector3Int, TileInfo>(data.tiles);

        if (data.enemyRoad != null) enemyRoad = data.enemyRoad;
        else enemyRoad = new List<Vector3Int>();

        mapProperties = data.mapProperties;
        tileProperties = data.tileProperties;

        rounds = data.rounds;
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

#region Tile

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

// 스프라이트 형태 표기용 룰타일.
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
#endregion

[Serializable]
public class MapProperty
{
    public Element element;
    public int atk;
    public int atkSpeed;
    public int hp;
    public int speed;

    public MapProperty()
    {
        element = Element.FIRE;
        atk = 0;
        atkSpeed = 0;
        hp = 0;
        speed = 0;
    }

    public MapProperty(MapProperty prop)
    {
        element = prop.element;
        atk = prop.atk;
        atkSpeed = prop.atkSpeed;
        hp = prop.hp;
        speed = prop.speed;
    }
}

[Serializable]
public class TileProperty
{
    public int atk;
    public int atkSpeed;
    public int range;

    public TileProperty()
    {
        atk = 0;
        atkSpeed = 0;
        range = 0;
    }
    public TileProperty(TileProperty prop)
    {
        atk = prop.atk;
        atkSpeed = prop.atkSpeed;
        range = prop.range;
    }
}

[Serializable]
public class Round : ISerializationCallbackReceiver
{
    public Dictionary<int, int> unitData;
    public List<int> units = new List<int>();
    public List<int> amounts = new List<int>();

    public void OnBeforeSerialize()
    {
        units.Clear();
        amounts.Clear();

        foreach (var kvp in unitData)
        {
            units.Add(kvp.Key);
            amounts.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        unitData = new Dictionary<int, int>();
        for (int i = 0; i < units.Count && i < amounts.Count; i++)
        {
            unitData.Add(units[i], amounts[i]);
        }
    }
}

// 실 사용할 데이터와 직렬화할 데이터를 구분
[Serializable]
public class MapDataJson : ISerializationCallbackReceiver
{
    public string backgroundName;
    public Vector3Int origin;
    public Vector3Int size;
    public List<Vector3Int> tileKeys = new List<Vector3Int>();
    public List<TileInfo> tileValues = new List<TileInfo>();
    public Dictionary<Vector3Int, TileInfo> tiles;
    public List<Vector3Int> enemyRoad;

    public List<Vector3Int> tpKeys = new List<Vector3Int>();
    public List<TileProperty> tpValues = new List<TileProperty>();
    public Dictionary<Vector3Int, TileProperty> tileProperties;
    public List<MapProperty> mapProperties;

    public List<Round> rounds;

    public MapDataJson(Map data)
    {
        backgroundName = data.backgroundName;
        origin = data.origin;
        size = data.size;
        tiles = data.tiles;
        enemyRoad = data.enemyRoad;
        mapProperties = data.mapProperties;
        tileProperties = data.tileProperties;
        rounds = data.rounds;
    }

    public void OnBeforeSerialize()
    {
        tileKeys.Clear();
        tileValues.Clear();

        tpKeys.Clear();
        tpValues.Clear();

        foreach (var kvp in tiles)
        {
            tileKeys.Add(kvp.Key);
            tileValues.Add(kvp.Value);
        }

        foreach (var kvp in tileProperties)
        {
            tpKeys.Add(kvp.Key);
            tpValues.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        tiles = new Dictionary<Vector3Int, TileInfo>();
        for (int i = 0; i < tileKeys.Count && i < tileValues.Count; i++)
        {
            tiles.Add(tileKeys[i], tileValues[i]);
        }

        tileProperties = new Dictionary<Vector3Int, TileProperty>();
        for (int i = 0; i < tpKeys.Count && i < tpValues.Count; i++)
        {
            tileProperties.Add(tpKeys[i], tpValues[i]);
        }
    }
}
