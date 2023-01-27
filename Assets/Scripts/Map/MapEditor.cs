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

    [SerializeField] private Tilemap mapTilemap;
    [SerializeField] private Tilemap buildableTilemap;
    [SerializeField] private Tilemap routeTilemap;
    [SerializeField] private SpriteRenderer selectedTile;
    private Grid grid;
    private Map map;
    public string MapName { get { return mapName; } }
    private string mapName;
    public List<Vector3Int> EnemyRoad { get { return road; } }
    private List<Vector3Int> road;
    private CustomTile selectedTileInfo;
    #region Map Controller
    public void Init(Map map, string mapName)
    {
        this.mapName = mapName;
        if (map == null)
        {
            Clear();
            return;
        }

        Clear();
        mapTilemap.origin = map.tilemap.origin;
        mapTilemap.size = map.tilemap.size;

        foreach (var pos in map.tilemap.tiles.Keys)
        {
            TileInfo tileInfo = map.tilemap.tiles[pos];
            TileBase tile = TileManager.GetTile(tileInfo.name);
            mapTilemap.SetTile(pos, tile);
        }

        TilemapInfo info = MakeMap();
        FindRoute(info);
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
            if (click && selectedTileInfo != null)
            {
                mapTilemap.SetTile(tilePos, selectedTileInfo);
                UpdateMap();
            }

            if (rclick)
            {
                mapTilemap.SetTile(tilePos, null);
                UpdateMap();
            }
        }
    }

    public bool SelectTile(Vector3 worldPos)
    {
        selectedTile.gameObject.SetActive(true);

        // 굳이 WorldPos를 GetTilePos를 한번 더 거쳐주는 이유는 작업의 실수를 최소화 하기 위함임.
        // 실제 위치와 타일의 위치가 약간 다름.
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = (map != null && map.tilemap.Buildable(tilePos));

        if (buildable) selectedTile.color = new Color(0, 1, 0, 0.7f);
        else selectedTile.color = new Color(1, 0, 0, 0.7f);

        // 선택된 타일의 위치를 localPosition으로 조정하지 않으면 grid에서 위치가 어긋남.
        selectedTile.transform.localPosition = pos;

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
    public void SelectTile(CustomTile tile)
    {
        selectedTileInfo = tile;
        selectedTile.sprite = tile.sprite;
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
    public bool FindRoute(TilemapInfo tilemap)
    {
        road = MapManager.FindRoute(tilemap);

        if (road == null) return false;

        routeTilemap.ClearAllTiles();
        UpdateMap();
        for (int i = 0; i < road.Count; i++)
        {
            if (i == 0) routeTilemap.SetTile(road[i], TileManager.GetTile("STARTFLAG"));
            else if (i == road.Count - 1) routeTilemap.SetTile(road[i], TileManager.GetTile("DESTFLAG"));
            else routeTilemap.SetTile(road[i], TileManager.GetTile("CORNER"));
        }
        return true;
    }

    // 빌드 가능한 곳을 단순히 보여주는 용도
    public void UpdateMap()
    {
        buildableTilemap.ClearAllTiles();

        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    bool b = (tn != "WALL" && tn != "ROAD" && tn != "START" && tn != "DEST");
                    buildableTilemap.SetTile(pos, (b) ? TileManager.GetTile("BUILDABLE") : TileManager.GetTile("NOTBUILDABLE"));
                }
            }
        }
    }

    public TilemapInfo MakeMap()
    {
        UpdateMap();

        Dictionary<Vector3Int, TileInfo> mapInfo = new Dictionary<Vector3Int, TileInfo>();
        for (int y = mapTilemap.size.y; y >= 0; y--)
        {
            for (int x = 0; x < mapTilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y) + mapTilemap.origin;
                TileBase tile = mapTilemap.GetTile(pos);
                if (tile != null)
                {
                    string tn = tile.name.ToUpper();
                    bool b = (tn != "WALL" && tn != "ROAD" && tn != "START" && tn != "DEST");
                    mapInfo.Add(pos, new TileInfo(tile.name, b));
                }
            }
        }

        TilemapInfo tilemap = new TilemapInfo(mapTilemap.origin, mapTilemap.size, mapInfo);

        if (FindRoute(tilemap) == false)
        {
#if UNITY_EDITOR
            Debug.Log("[SYSTEM] CAN NOT MAKE MAP");
#endif
            return null;
        }
        return tilemap;
    }
}
