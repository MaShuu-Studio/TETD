using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Data;
using System.Threading.Tasks;

public static class TileManager
{
    private static Dictionary<string, CustomTile> tiles;

    private static string path = "/Tile/";
    public static async Task Init()
    {
        tiles = new Dictionary<string, CustomTile>();
        List<string> fileNames = DataManager.GetFiles(Application.streamingAssetsPath + "/Sprites" + path, ".png");
        for (int i = 0; i < fileNames.Count; i++)
        {
            string name = fileNames[i].ToUpper();
            Sprite sprite = await DataManager.LoadSprite(path + name + ".png", Vector2.one / 2, 16);

            if (sprite == null) continue;

            CustomTile tile = ScriptableObject.CreateInstance<CustomTile>();
            tile.SetData(name, sprite);
            tiles.Add(name, tile);
        }

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
