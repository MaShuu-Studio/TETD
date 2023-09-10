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
    // 0: buildable, 1: notbuildable, 2: roads
    private static Dictionary<string, CustomRuleTile>[] tiles;
    private static Dictionary<string, Sprite[]> backgrounds;

    public static string[] FlagNames { get { return flagNames; } }
    public static string[] BackgroundNames { get { return backgroundNames; } }
    private static string[] backgroundNames;

    private static string tilePath = "/Sprites/Tile/";
    private static string flagPath = "/Sprites/Tile/Flag/";
    private static string backgroundPath = "/Sprites/Tile/Backgrounds/";

    private static string[] flagNames = { "BUILDABLE", "NOTBUILDABLE", "STARTFLAG", "DESTFLAG", "CORNER" };
    private static string[] tileTypes = { "BUILDABLE", "NOT BUILDABLE", "ROAD" };
    private static string[] mapBackgrounds = { "LOWER", "UPPER" };

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static void GetTotal()
    {
        List<string> files = new List<string>();
        foreach (var path in tileTypes)
        {
            files.AddRange(DataManager.GetFileNames(tilePath + "/" + path));
        }
        string[] backgroundNames = DataManager.GetDics(Application.streamingAssetsPath + backgroundPath);

        TotalProgress = flagNames.Length + files.Count + backgroundNames.Length;
    }

    public static async void Init()
    {
        flags = new Dictionary<string, CustomRuleTile>();

        for (int i = 0; i < flagNames.Length; i++)
        {
            CurProgress++;
            string path = flagPath + flagNames[i] + ".png";
            string tag = "FLAG";
            if (flagNames[i].Contains("BUILDABLE")) tag = "BUILDABLE";
            CustomRuleTile tile = await DataManager.LoadTile(path, flagNames[i], tag, false);
            if (tile == null) continue; // 게임 실행에 오류가 생기므로 아예 게임을 종료시키는게 나음.

            flags.Add(flagNames[i], tile);
        }

        tiles = new Dictionary<string, CustomRuleTile>[3];

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Dictionary<string, CustomRuleTile>();
            string filePath = tilePath + "/" + tileTypes[i] + "/";

            List<string> fileNames = DataManager.GetFileNames(filePath);
            bool buildable = (i == 0); // buildable의 경우에만 true
            for (int j = 0; j < fileNames.Count; j++)
            {
                CurProgress++;
                string name = DataManager.FileNameTriming(fileNames[j]).ToUpper();
                CustomRuleTile tile = await DataManager.LoadTile(filePath + fileNames[j], name, tileTypes[i], buildable);
                if (tile == null) continue;

                tiles[i].Add(name, tile);
            }
        }

        backgrounds = new Dictionary<string, Sprite[]>();
        string[] names = DataManager.GetDics(Application.streamingAssetsPath + backgroundPath);

        for (int i = 0; i < names.Length; i++)
        {
            CurProgress++;
            Sprite[] b = new Sprite[mapBackgrounds.Length];
            string name = names[i].ToUpper();
            for (int j = 0; j < mapBackgrounds.Length; j++)
            {
                b[j] = await DataManager.LoadSprite(backgroundPath + names[i] + "/" + mapBackgrounds[j] + ".png", Vector2.one / 2, 24);
            }
            backgrounds.Add(name, b);
        }

        backgroundNames = backgrounds.Keys.ToArray();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TILE {tiles[0].Count + tiles[1].Count + tiles[2].Count}");
        Debug.Log($"[SYSTEM] LOAD BACKGROUND {backgrounds.Count}");
        Debug.Log($"[SYSTEM] LOAD TILE FLAG {flags.Count}");
#endif
    }

    public static List<CustomRuleTile>[] GetTiles()
    {
        List<CustomRuleTile>[] list = new List<CustomRuleTile>[3];
        list[0] = tiles[0].Values.ToList();
        list[1] = tiles[1].Values.ToList();
        list[2] = tiles[2].Values.ToList();
        return list;
    }

    public static CustomRuleTile GetTile(TileInfo tileInfo)
    {
        CustomRuleTile tile = null;
        string tileName = tileInfo.name.ToUpper();
        if (flags.ContainsKey(tileName)) tile = flags[tileName];
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].ContainsKey(tileName))
            {
                tile = tiles[i][tileName];
            }
        }

        return tile;
    }
    public static CustomRuleTile GetTile(string tileName)
    {
        CustomRuleTile tile = null;
        if (flags.ContainsKey(tileName)) tile = flags[tileName];
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].ContainsKey(tileName))
            {
                tile = tiles[i][tileName];
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
        if (string.IsNullOrEmpty(name)) return null;

        name = name.ToUpper();
        if (backgrounds.ContainsKey(name)) return backgrounds[name];

        return null;
    }
}
