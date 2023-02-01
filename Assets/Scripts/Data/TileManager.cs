using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Data;
using System.Threading.Tasks;

public static class TileManager
{
    private static Dictionary<string, CustomTile> flags;
    private static Dictionary<string, TilePalette> tiles;
    public static List<string> TilePaletteNames { get { return tilePaletteNames; } }
    private static List<string> tilePaletteNames;

    private static string mapPath = "/Sprites/Tile/Maps/";
    private static string flagPath = "/Sprites/Tile/Flag/";

    private static string[] flagNames = { "BUILDABLE", "NOTBUILDABLE", "STARTFLAG", "DESTFLAG", "CORNER", "HORIZONTAL", "VERTICAL" };
    private static string[] tileDics = { "/Buildable/", "/Not Buildable/" };
    private static string[] constTiles = { "ROAD", "START", "DEST" };
    public static async Task Init()
    {
        flags = new Dictionary<string, CustomTile>();
        for (int i = 0; i < flagNames.Length; i++)
        {
            CustomTile tile = await InitTile(flagPath + flagNames[i] + ".png", DataManager.FileNameTriming(flagNames[i]));
            if (tile == null) continue;

            flags.Add(flagNames[i], tile);
        }

        tiles = new Dictionary<string, TilePalette>();
        string[] tilePalettes = DataManager.GetDics(Application.streamingAssetsPath + mapPath);
        for (int i = 0; i < tilePalettes.Length; i++)
        {
            string filePath = mapPath + tilePalettes[i];
            Dictionary<string, CustomTile> buildable = new Dictionary<string, CustomTile>();
            List<string> fileNames = DataManager.GetFileNames(filePath + tileDics[0]);
            for (int j = 0; j < fileNames.Count; j++)
            {
                string name = DataManager.FileNameTriming(fileNames[j]);
                CustomTile tile = await InitTile(filePath + tileDics[0] + fileNames[j], name);
                if (tile == null) continue;

                buildable.Add(name, tile);
            }
            Dictionary<string, CustomTile> notBuildable = new Dictionary<string, CustomTile>();
            fileNames = DataManager.GetFileNames(filePath + tileDics[1]);
            for (int j = 0; j < fileNames.Count; j++)
            {
                string name = DataManager.FileNameTriming(fileNames[j]);
                CustomTile tile = await InitTile(filePath + tileDics[1] + fileNames[j], name);
                if (tile == null) continue;

                notBuildable.Add(name, tile);
            }
            Dictionary<string, CustomTile> roads = new Dictionary<string, CustomTile>();
            for (int j = 0; j < constTiles.Length; j++)
            {
                CustomTile tile = await InitTile(filePath + "/" + constTiles[j] + ".png", constTiles[j]);
                if (tile == null) continue;

                roads.Add(constTiles[j], tile);
            }

            string tilePaletteName = tilePalettes[i].ToUpper();

            TilePalette tilePalette = new TilePalette(tilePalettes[i], buildable, notBuildable, roads);
            tiles.Add(tilePaletteName, tilePalette);
        }
        tilePaletteNames = tiles.Keys.ToList();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TILEMAP {tiles.Count}");
        Debug.Log($"[SYSTEM] LOAD TILE FLAG {flags.Count}");
#endif
    }

    private static async Task<CustomTile> InitTile(string path, string name)
    {
        name = name.ToUpper();
        Sprite sprite = await DataManager.LoadSprite(path, Vector2.one / 2, 16);

        if (sprite == null) return null;

        sprite.name = name;
        CustomTile tile = ScriptableObject.CreateInstance<CustomTile>();
        tile.SetData(name, sprite);

        return tile;
    }

    public static TilePalette GetTilePalette(string tilePaletteName)
    {
        if (tiles.ContainsKey(tilePaletteName)) return tiles[tilePaletteName];
        return null;
    }

    public static CustomTile GetTile(string tilePaletteName, TileInfo tileInfo)
    {
        tilePaletteName = tilePaletteName.ToUpper();
        CustomTile tile = null;
        if (tiles.ContainsKey(tilePaletteName))
        {
            string tileName = tileInfo.name.ToUpper();
            if (tiles[tilePaletteName].roads.ContainsKey(tileName))
            {
                tile = tiles[tilePaletteName].roads[tileName];
            }
            else if (tileInfo.buildable && tiles[tilePaletteName].buildable.ContainsKey(tileName))
            {
                tile = tiles[tilePaletteName].buildable[tileName];
            }
            else if (tileInfo.buildable == false && tiles[tilePaletteName].notBuildable.ContainsKey(tileName))
            {
                tile = tiles[tilePaletteName].notBuildable[tileName];
            }
        }

        return tile;
    }

    public static CustomTile GetFlag(string name)
    {
        name = name.ToUpper();
        if (flags.ContainsKey(name)) return flags[name];

        return null;
    }
}
