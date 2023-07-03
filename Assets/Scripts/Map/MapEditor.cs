using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapEditor : MonoBehaviour
{
    public static MapEditor Instance { get { return instance; } }
    private static MapEditor instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        grid = GetComponent<Grid>();

        cam = FindObjectOfType<Camera>();
        pcam = cam.GetComponent<PixelPerfectCamera>();

        float ratio = (float)1920 / Screen.width;

        pcam.assetsPPU = Mathf.CeilToInt(81 / ratio);
        pcam.refResolutionX = Screen.width;
        pcam.refResolutionY = Screen.height;

        transform.position = Vector3.zero;
    }
    private Camera cam;
    private PixelPerfectCamera pcam;

    [SerializeField] private SpriteRenderer[] backgrounds;

    [SerializeField] private Tilemap mapTilemap;
    [SerializeField] private Tilemap buildableTilemap;
    [SerializeField] private Tilemap routeTilemap;
    [SerializeField] private SpriteRenderer selectedPosSprite;
    private Grid grid;
    private Map map;

    public string MapName { get { return mapName; } }
    private string mapName;

    public TilemapInfo Tilemap { get { return tilemap; } }
    private TilemapInfo tilemap;

    public List<Vector3Int> EnemyRoad { get { return road; } }
    private List<Vector3Int> road;
    private CustomRuleTile selectedTile;
    private CustomRuleTile roadTile;

    private bool start, dest;
    private Vector3Int startPos, destPos;
    private bool drawFlag;

    public bool CanSave { get { return canSave; } }
    private bool canSave;

    #region Map Controller
    public void Init(Map map, string mapName)
    {
        Clear();

        this.mapName = mapName;

        roadTile = TileManager.GetTiles()[2][0]; // Road에 대한 임시조치.

        if (map == null) tilemap = new TilemapInfo("");
        else tilemap = new TilemapInfo(map.tilemap);

        Sprite[] sprites = tilemap.GetBackGround();
        backgrounds[0].gameObject.SetActive(false);
        backgrounds[1].gameObject.SetActive(false);
        if (sprites != null)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (sprites[i] == null) continue;

                // 전체사이즈가 640*360일 때 딱 맞게 되어있음.
                // 이에 맞춰서 사이즈 조정

                float x, y;
                x = sprites[i].texture.width;
                y = sprites[i].texture.height;

                backgrounds[i].transform.localScale = new Vector3(640 / x, 360 / y);
                backgrounds[i].sprite = sprites[i];
                backgrounds[i].gameObject.SetActive(true);
            }
        }

        UpdateMap();
        FindRoute();
    }

    // Update is called once per frame
    private void Update()
    {
        bool click = Input.GetMouseButton(0);
        bool rclick = Input.GetMouseButton(1);

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = SelectTile(worldPos);

        if (UIController.Instance.PointInTilePanel(mousePos) == false)
        {
            if (click && selectedTile != null)
            {
                SetTile(tilePos, selectedTile);

                if (selectedTile.name == "START")
                {
                    if (start) SetTile(startPos, roadTile);
                    start = true;
                    startPos = tilePos;
                }
                if (selectedTile.name == "DEST")
                {
                    if (dest) SetTile(destPos, roadTile);
                    dest = true;
                    destPos = tilePos;
                }

                FindRoute();
            }

            if (rclick)
            {
                SetTile(tilePos, null);
                FindRoute();
            }
        }
    }

    public bool SelectTile(Vector3 worldPos)
    {
        selectedPosSprite.gameObject.SetActive(true);

        // 굳이 WorldPos를 GetTilePos를 한번 더 거쳐주는 이유는 작업의 실수를 최소화 하기 위함임.
        // 실제 위치와 타일의 위치가 약간 다름.
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = (map != null && map.tilemap.Buildable(tilePos));

        if (buildable) selectedPosSprite.color = new Color(0, 1, 0, 0.7f);
        else selectedPosSprite.color = new Color(1, 0, 0, 0.7f);

        // 선택된 타일의 위치를 localPosition으로 조정하지 않으면 grid에서 위치가 어긋남.
        selectedPosSprite.transform.localPosition = pos;

        return buildable;
    }

    public Vector3 GetMapPos(Vector3 pos)
    {
        /* 타일은 grid.cellSize만큼 나누어져 있음
         * 따라서 worldPos에서 내림을 통해 사각형의 왼쪽 아래 꼭지점을 얻음.
         * 이후 cellSize의 절반만큼 위치를 조정해주면 타일의 위치가 됨.
         */
        float x = Mathf.FloorToInt(pos.x) + grid.cellSize.x / 2;
        float y = Mathf.FloorToInt(pos.y) + grid.cellSize.y / 2;

        return new Vector3(x, y);
    }
    #endregion
    public void SelectTile(CustomRuleTile tile, bool isFlag)
    {
        selectedTile = tile;
        selectedPosSprite.sprite = tile.Base.sprite;
        drawFlag = isFlag;
    }

    #region Map
    private void SetTile(Vector3Int pos, CustomRuleTile tile)
    {
        if (drawFlag)
        {
            routeTilemap.SetTile(pos, tile.Base);
        }
        else
        {
            bool update = true;
            if (tilemap.tiles.ContainsKey(pos))
            {
                if (tilemap.tiles[pos].name == "START") start = false;
                if (tilemap.tiles[pos].name == "DEST") dest = false;

                if (tile == null)
                {
                    tilemap.tiles.Remove(pos);
                }
                else if (tilemap.tiles[pos].name != tile.name)
                    tilemap.tiles[pos] = new TileInfo(tile.name, tile.Base.buildable);
                else update = false;
            }
            else
            {
                if (tilemap.tiles.Count == 0)
                {
                    tilemap.origin = pos;
                    tilemap.size = new Vector3Int(1, 1);
                }
                if (tile != null)
                    tilemap.tiles.Add(pos, new TileInfo(tile.name, tile.Base.buildable));
            }

            int x = tilemap.origin.x;
            int y = tilemap.origin.y;

            if (x > pos.x)
            {
                tilemap.size.x += x - pos.x;
                tilemap.origin.x = pos.x;
            }
            else if (x + (tilemap.size.x - 1) < pos.x)
            {
                tilemap.size.x = pos.x - x + 1;
            }

            if (y > pos.y)
            {
                tilemap.size.y += y - pos.y;
                tilemap.origin.y = pos.y;
            }
            else if (y + (tilemap.size.y - 1) < pos.y)
            {
                tilemap.size.y = pos.y - y + 1;
            }

            if (update) UpdateMap(pos);
        }
    }
    public void Clear()
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();
        routeTilemap.ClearAllTiles();
    }
    public void ClearInfo()
    {
        buildableTilemap.ClearAllTiles();
        routeTilemap.ClearAllTiles();
    }

    // 루트를 보여주는 용도
    public void FindRoute()
    {
        if (start == false) return;

        Tuple<bool, List<Vector3Int>> result = MapManager.FindRoute(tilemap);

        canSave = result.Item1;
        road = result.Item2;

        routeTilemap.ClearAllTiles();
        routeTilemap.SetTile(road[0], TileManager.GetFlag("STARTFLAG").Base);
        for (int i = 1; i < road.Count; i++)
        {
            Vector3Int dir = (road[i] - road[i - 1]);
            CustomTile line = TileManager.GetFlag("CORNER").Base;
            if (dir.x == 0) line = TileManager.GetFlag("CORNER").Base;

            if (dir.x < 0) dir.x = -1;
            else if (dir.x > 0) dir.x = 1;
            if (dir.y < 0) dir.y = -1;
            else if (dir.y > 0) dir.y = 1;

            Vector3Int pos = road[i - 1] + dir;
            while (pos != road[i])
            {
                routeTilemap.SetTile(pos, line);
                pos += dir;
            }

            routeTilemap.SetTile(road[i], TileManager.GetFlag("CORNER").Base);
        }
        if (canSave) routeTilemap.SetTile(road[road.Count - 1], TileManager.GetFlag("DESTFLAG").Base);
        else
        {
#if UNITY_EDITOR
            Debug.Log("[SYSTEM] CAN NOT MAKE MAP");
#endif
        }
    }

    public void UpdateMap(Vector3Int selectedPos)
    {
        Vector3Int[] dir = new Vector3Int[5] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.zero };

        for (int i = 0; i < dir.Length; i++)
        {
            Vector3Int pos = selectedPos + dir[i];
            CustomTile tile = null;
            CustomTile buildableFlag = null;

            CustomRuleTile ruleTile = tilemap.GetTile(pos);
            if (ruleTile != null)
            {
                string[] info = new string[4] { "", "", "", "" };

                for (int j = 0; j < 4; j++)
                {
                    CustomRuleTile aroundTile = tilemap.GetTile(pos + dir[j]);
                    if (aroundTile != null) info[j] = aroundTile.name;
                }

                tile = ruleTile.GetTile(info);
                buildableFlag = (tile.buildable) ? TileManager.GetFlag("BUILDABLE").Base : TileManager.GetFlag("NOTBUILDABLE").Base;
            }

            mapTilemap.SetTile(pos, tile);
            buildableTilemap.SetTile(pos, buildableFlag);
        }
    }

    // 전체 맵을 업데이트
    public void UpdateMap()
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();

        start = false;
        dest = false;

        foreach (var pos in tilemap.tiles.Keys)
        {
            CustomRuleTile ruleTile = tilemap.GetTile(pos);
            if (ruleTile != null)
            {
                string[] info = new string[4] { "", "", "", "" };
                Vector3Int[] dir = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                for (int i = 0; i < 4; i++)
                {
                    CustomRuleTile aroundTile = tilemap.GetTile(pos + dir[i]);
                    if (aroundTile != null) info[i] = aroundTile.name;
                }

                CustomTile tile = ruleTile.GetTile(info);
                bool b = tile.buildable;

                mapTilemap.SetTile(pos, tile);
                buildableTilemap.SetTile(pos, (b) ? TileManager.GetFlag("BUILDABLE").Base : TileManager.GetFlag("NOTBUILDABLE").Base);

                string tn = tile.name.ToUpper();
                if (tn == "START")
                {
                    startPos = pos;
                    start = true;
                }
                if (tn == "DEST")
                {
                    destPos = pos;
                    dest = true;
                }
            }
        }
    }
    #endregion
}
