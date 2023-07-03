using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Data;
using System.Threading.Tasks;

public static class TileManager
{
    private static Dictionary<string, CustomRuleTile> flags;
    private static Dictionary<string, TilePalette> tiles;

    public static string[] FlagNames { get { return flagNames; } }
    public static List<string> TilePaletteNames { get { return tilePaletteNames; } }
    private static List<string> tilePaletteNames;

    private static string mapPath = "/Sprites/Tile/Maps/";
    private static string flagPath = "/Sprites/Tile/Flag/";

    private static string[] flagNames = { "BUILDABLE", "NOTBUILDABLE", "STARTFLAG", "DESTFLAG", "CORNER" };
    private static string[] tileDics = { "/Buildable/", "/Not Buildable/" };
    private static string[] constTiles = { "ROAD", "START", "DEST" };
    private static string[] mapBackgrounds = { "LOWER", "UPPER" };

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static bool IsConstTile(string str)
    {
        for (int i = 0; i < constTiles.Length; i++)
        {
            if (str == constTiles[i]) return true;
        }
        return false;
    }

    public static void GetTotal()
    {
        string[] tilePalettes = DataManager.GetDics(Application.streamingAssetsPath + mapPath);

        TotalProgress = flagNames.Length + tilePalettes.Length;
    }

    public static async Task Init()
    {
        flags = new Dictionary<string, CustomRuleTile>();
        tiles = new Dictionary<string, TilePalette>();
        string[] tilePalettes = DataManager.GetDics(Application.streamingAssetsPath + mapPath);

        for (int i = 0; i < flagNames.Length; i++)
        {
            string path = flagPath + flagNames[i] + ".png";
            CustomRuleTile tile = await DataManager.LoadTile(path, flagNames[i], false);
            if (tile == null) continue; // 게임 실행에 오류가 생기므로 아예 게임을 종료시키는게 나음.

            flags.Add(flagNames[i], tile);

            CurProgress++;
        }

        for (int i = 0; i < tilePalettes.Length; i++)
        {
            string filePath = mapPath + tilePalettes[i];

            List<string> fileNames = DataManager.GetFileNames(filePath + tileDics[0]);
            Dictionary<string, CustomRuleTile> buildableTiles = new Dictionary<string, CustomRuleTile>();
            bool buildable = true;
            for (int j = 0; j < fileNames.Count; j++)
            {
                string name = DataManager.FileNameTriming(fileNames[j]);
                CustomRuleTile tile = await DataManager.LoadTile(filePath + tileDics[0] + fileNames[j], name, buildable);
                if (tile == null) continue;

                buildableTiles.Add(name, tile);
            }

            fileNames = DataManager.GetFileNames(filePath + tileDics[1]);
            Dictionary<string, CustomRuleTile> notBuildableTiles = new Dictionary<string, CustomRuleTile>();
            buildable = false;
            for (int j = 0; j < fileNames.Count; j++)
            {
                string name = DataManager.FileNameTriming(fileNames[j]);
                CustomRuleTile tile = await DataManager.LoadTile(filePath + tileDics[1] + fileNames[j], name, buildable);
                if (tile == null) continue;

                notBuildableTiles.Add(name, tile);
            }

            Dictionary<string, CustomRuleTile> roads = new Dictionary<string, CustomRuleTile>();
            for (int j = 0; j < constTiles.Length; j++)
            {
                CustomRuleTile tile = await DataManager.LoadTile(filePath + "/" + constTiles[j] + ".png", constTiles[j], buildable);
                if (tile == null) continue;

                roads.Add(constTiles[j], tile);
            }

            Sprite[] backgrounds = new Sprite[mapBackgrounds.Length];
            for (int j = 0; j < mapBackgrounds.Length; j++)
            {
                backgrounds[j] = await DataManager.LoadSprite(filePath + "/" + mapBackgrounds[j] + ".png", Vector2.one / 2, 24);
            }

            string tilePaletteName = tilePalettes[i].ToUpper();

            TilePalette tilePalette =
                new TilePalette(
                    tilePalettes[i],
                    buildableTiles,
                    notBuildableTiles,
                    roads,
                    backgrounds);
            tiles.Add(tilePaletteName, tilePalette);

            CurProgress++;
        }
        tilePaletteNames = tiles.Keys.ToList();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TILEMAP {tiles.Count}");
        Debug.Log($"[SYSTEM] LOAD TILE FLAG {flags.Count}");
#endif
    }

    public static TilePalette GetTilePalette(string tilePaletteName)
    {
        if (tiles.ContainsKey(tilePaletteName)) return tiles[tilePaletteName];
        return null;
    }

    public static List<CustomRuleTile>[] GetTiles(string tilePaletteName)
    {
        if (tiles.ContainsKey(tilePaletteName))
        {
            List<CustomRuleTile>[] list = new List<CustomRuleTile>[3];
            list[0] = tiles[tilePaletteName].Buildable;
            list[1] = tiles[tilePaletteName].NotBuildable;
            list[2] = tiles[tilePaletteName].Roads;
            return list;
        }
        return null;
    }

    public static CustomRuleTile GetTile(string tilePaletteName, TileInfo tileInfo)
    {
        tilePaletteName = tilePaletteName.ToUpper();
        CustomRuleTile tile = null;
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

    public static CustomRuleTile GetFlag(string name)
    {
        name = name.ToUpper();
        if (flags.ContainsKey(name)) return flags[name];

        return null;
    }

    public static Sprite[] GetBackground(string name)
    {
        name = name.ToUpper();
        if (tiles.ContainsKey(name)) return tiles[name].backgrounds;

        return null;
    }
}
