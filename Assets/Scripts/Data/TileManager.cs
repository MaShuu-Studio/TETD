using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Data;
using System.Threading.Tasks;

public static class TileManager
{
    private static Dictionary<string, CustomTile> tiles;
    public static List<string> Keys { get { return keys; } }
    private static List<string> keys;

    private static string path = "/Tile/";
    public static async Task Init()
    {
        tiles = new Dictionary<string, CustomTile>();
        string[] fileNames = await DataManager.GetFiles(Application.streamingAssetsPath + "/Sprites" + path);

        for (int i = 0; i < fileNames.Length; i++)
        {
            Sprite sprite = await DataManager.LoadSprite(fileNames[i], Vector2.one / 2, 16);

            if (sprite == null) continue;

            string name = DataManager.FileNameTriming(fileNames[i]).ToUpper();
            sprite.name = name;
            CustomTile tile = ScriptableObject.CreateInstance<CustomTile>();
            tile.SetData(name, sprite);
            tiles.Add(name, tile);
        }
        keys = tiles.Keys.ToList();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TILE {tiles.Count}");
#endif
    }

    public static CustomTile GetTile(string name)
    {
        name = name.ToUpper();
        if (tiles.ContainsKey(name)) return tiles[name];

        return null;
    }
}
