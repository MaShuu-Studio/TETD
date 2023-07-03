using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorPanel : MonoBehaviour
{
    [SerializeField] private MapEditorTile tilePrefab;
    [SerializeField] private MapEditorBackgroundTile backgroundTilePrefab;
    // 0: Buildable, 1: Not Buildable, 2: Route Flag
    [SerializeField] private Transform[] toggles;
    [SerializeField] private GameObject[] palettes;
    // 0: Buildable, 1: Not Buildable, 2: Road, 3: Route Flag, 4: Backgrounds
    [SerializeField] private Transform[] tileLists;
    private List<MapEditorTile>[] tiles;
    private List<MapEditorBackgroundTile> backgrounds;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tiles = new List<MapEditorTile>[4];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new List<MapEditorTile>();
        backgrounds = new List<MapEditorBackgroundTile>();
    }

    public void Init()
    {
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
                    tiles[i].Add(metile);
                }
            }
        }

        // BUILDABLE, NOTBUILDABLE 제외.
        for (int i = 2; i < TileManager.FlagNames.Length; i++)
        {
            CustomRuleTile tile = TileManager.GetFlag(TileManager.FlagNames[i]);
            MapEditorTile metile = Instantiate(tilePrefab);

            metile.SetTile(tile, true);
            metile.transform.SetParent(tileLists[3]);
            tiles[3].Add(metile);
        }

        // Background Tiles
        for (int i = 0; i < TileManager.BackgroundNames.Length; i++)
        {
            string name = TileManager.BackgroundNames[i];
            Sprite[] bg = TileManager.GetBackground(name);
            MapEditorBackgroundTile bgTile = Instantiate(backgroundTilePrefab);

            bgTile.SetTile(name, bg);
            bgTile.transform.SetParent(tileLists[4]);
            backgrounds.Add(bgTile);
        }

        LoadPalette(0);
    }

    public bool PointInTilePanel(Vector2 point)
    {
        Vector2 pos = rectTransform.position;
        Rect rect = new Rect(pos + rectTransform.offsetMin, rectTransform.rect.size);
        if (rect.Contains(point)) return true;
        return false;
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
