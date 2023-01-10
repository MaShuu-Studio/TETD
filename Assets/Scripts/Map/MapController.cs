using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapController : MonoBehaviour
{
    public static MapController Instance { get { return instance; } }
    private static MapController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private GameObject selectedTile;
    [SerializeField] private Tilemap tilemap;
    public Grid grid;
    public Map map;

    // 임시 변수
    private Camera cam;
    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        SelectTile();
    }

    public void SelectTile()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        // 선택된 타일의 위치를 localPosition으로 조정하지 않으면 grid에서 위치가 어긋남.
        selectedTile.transform.localPosition = GetTilePos(worldPos);
    }

    public Vector2 GetTilePos(Vector2 pos)
    {
        /* 타일은 grid.cellSize만큼 나누어져 있음
         * 따라서 worldPos에서 내림을 통해 사각형의 왼쪽 아래 꼭지점을 얻음.
         * 이후 cellSize의 절반만큼 위치를 조정해주면 타일의 위치가 됨.
         */
        float x = (int)Mathf.Floor(pos.x) + grid.cellSize.x / 2;
        float y = (int)Mathf.Floor(pos.y) + grid.cellSize.y / 2;

        return new Vector2(x, y);
    }

    public void LoadMap(Map map)
    {
        if (map == null) return;

        this.map = map;

        tilemap.ClearAllTiles();
        tilemap.origin = map.tilemap.origin;
        tilemap.size = map.tilemap.size;

        foreach (var pos in map.tilemap.tiles.Keys)
        {
            TileInfo tile = map.tilemap.tiles[pos];
            tilemap.SetTile(pos, tile.tile);
        }
    }
}
