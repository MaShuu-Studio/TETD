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

    [SerializeField] private SpriteRenderer selectedTile;
    [SerializeField] private Tilemap tilemap;
    public Grid grid;
    private Map map;

    private void Start()
    {
        transform.position = Vector3.zero;
        OffSeletedTile();
    }

    public Map GetMap()
    {
        return map;
    }

    public void OffSeletedTile()
    {
        selectedTile.gameObject.SetActive(false);
    }

    public bool SelectTile(Vector3 worldPos)
    {
        selectedTile.gameObject.SetActive(true);

        // 굳이 WorldPos를 GetTilePos를 한번 더 거쳐주는 이유는 작업의 실수를 최소화 하기 위함임.
        // 실제 위치와 타일의 위치가 약간 다름.
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = (map != null && map.tilemap.Buildable(tilePos));

        if (TowerController.Instance.ContainsTower(pos)) buildable = false;

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

    public void LoadMap(Map map)
    {
        if (map == null) return;

        this.map = map;

        tilemap.ClearAllTiles();
        tilemap.origin = map.tilemap.origin;
        tilemap.size = map.tilemap.size;

        foreach (var pos in map.tilemap.tiles.Keys)
        {
            TileInfo tileInfo = map.tilemap.tiles[pos];
            TileBase tile = TileManager.GetTile(tileInfo.name);
            tilemap.SetTile(pos, tile);
        }
    }
}
