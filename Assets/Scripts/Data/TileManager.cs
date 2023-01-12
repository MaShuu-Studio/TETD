using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileManager
{
    private static Dictionary<string, TileBase> tiles;
    public static void Init()
    {
        tiles = new Dictionary<string, TileBase>();
        List<TileBase> list = ResourceManager.GetResources<TileBase>("Tile");

        foreach (var tile in list)
        {
            tiles.Add(tile.name.ToUpper(), tile);
        }

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TILE {tiles.Count}");
#endif
    }

    public static TileBase GetTile(string name)
    {
        name = name.ToUpper();
        if (tiles.ContainsKey(name)) return tiles[name];

        return null;
    }

}
