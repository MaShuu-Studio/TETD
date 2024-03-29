using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTilePanel : MonoBehaviour
{
    [SerializeField] private MapEditorTile tilePrefab;
    [SerializeField] private MapEditorBackgroundTile backgroundTilePrefab;
    // 0: Buildable, 1: Not Buildable, 2: Road, 3: Backgrounds
    [SerializeField] private Transform[] toggles;
    [SerializeField] private GameObject[] palettes;
    // 0: Buildable, 1: Not Buildable, 2: Road, 3: Special Zone, 4: Route Flag, 5: Backgrounds
    [SerializeField] private Transform[] tileLists;
    private List<MapEditorTile>[] tiles;
    private List<MapEditorBackgroundTile> backgrounds;

    public void Init()
    {
        tiles = new List<MapEditorTile>[5];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new List<MapEditorTile>();
        backgrounds = new List<MapEditorBackgroundTile>();

        foreach (var list in tiles)
        {
            foreach (var tile in list)
            {
                Destroy(tile.gameObject);
            }
            list.Clear();
        }

        List<CustomRuleTile>[] tilelist = TileManager.GetTiles();
        if (tilelist != null)
        {
            // flag는 제외 따로 추가.
            for (int i = 0; i < tilelist.Length; i++)
            {
                for (int j = 0; j < tilelist[i].Count; j++)
                {
                    CustomRuleTile tile = tilelist[i][j];
                    MapEditorTile metile = Instantiate(tilePrefab);

                    metile.SetTile(tile);
                    metile.transform.SetParent(tileLists[i]);
                    metile.transform.localScale = Vector3.one;
                    tiles[i].Add(metile);
                }
            }
        }

        // SPECIAL ZONE 추가
        {
            CustomRuleTile tile = TileManager.GetFlag("SPECIALZONE");
            MapEditorTile metile = Instantiate(tilePrefab);

            metile.SetTile(tile, true);
            metile.transform.SetParent(tileLists[3]);
            metile.transform.localScale = Vector3.one;
            tiles[3].Add(metile);
        }

        // STARTFLAG와 DESTFLAG 추가
        string[] flags = { "STARTFLAG", "DESTFLAG" };
        for (int i = 0; i < flags.Length; i++)
        {
            CustomRuleTile tile = TileManager.GetFlag(flags[i]);
            MapEditorTile metile = Instantiate(tilePrefab);

            metile.SetTile(tile, true);
            metile.transform.SetParent(tileLists[4]);
            metile.transform.localScale = Vector3.one;
            tiles[4].Add(metile);
        }

        // Background Tiles
        for (int i = 0; i < TileManager.BackgroundNames.Length; i++)
        {
            string name = TileManager.BackgroundNames[i];
            Sprite[] bg = TileManager.GetBackground(name);
            MapEditorBackgroundTile bgTile = Instantiate(backgroundTilePrefab);

            bgTile.SetTile(name, bg);
            bgTile.transform.SetParent(tileLists[5]);
            bgTile.transform.localScale = Vector3.one;
            backgrounds.Add(bgTile);
        }

        LoadPalette(0);
    }

    public void LoadPalette(int index)
    {
        for (int i = 0; i < palettes.Length; i++)
        {
            if (i == index) toggles[i].SetAsLastSibling();
            else toggles[i].SetAsFirstSibling();

            palettes[i].SetActive(i == index);
        }
    }
}
