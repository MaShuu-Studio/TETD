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

        cam = FindObjectOfType<Camera>();

        transform.position = Vector3.zero;
        OffSeletedTile();
    }

    private Camera cam;
    [SerializeField] private SpriteRenderer selectedTile;
    [SerializeField] private Tilemap tilemap;
    public Grid grid;
    private Map map;

    private bool readyToBuild;
    private int id;

    public void Init(Map map)
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
    #region Tile
    // Update is called once per frame
    private void Update()
    {
        bool buildable = false;
        bool click = Input.GetMouseButtonDown(0);
        bool rclick = Input.GetMouseButtonDown(1);

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Vector3 pos = GetMapPos(worldPos);

        if (readyToBuild)
        {
            buildable = SelectTile(worldPos);
            if (click && buildable && TowerController.Instance.BuildTower(id, pos))
            {
                readyToBuild = false;
                id = 0;
            }
        }

        if (click
            && UIController.Instance.PointInTowerInfo(mousePos) == false
            && TowerController.Instance.SelectTower(pos))
        {
            OffSeletedTile();
            readyToBuild = false;
            id = 0;
        }

        if (rclick)
            TowerController.Instance.RemoveTower(pos);
    }

    public void ReadyToBuild(int id)
    {
        this.id = id;
        readyToBuild = true;
    }

    public void OffSeletedTile()
    {
        selectedTile.gameObject.SetActive(false);
    }

    public bool SelectTile(Vector3 worldPos)
    {
        selectedTile.gameObject.SetActive(true);

        // ���� WorldPos�� GetTilePos�� �ѹ� �� �����ִ� ������ �۾��� �Ǽ��� �ּ�ȭ �ϱ� ������.
        // ���� ��ġ�� Ÿ���� ��ġ�� �ణ �ٸ�.
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = (map != null && map.tilemap.Buildable(tilePos));

        if (TowerController.Instance.ContainsTower(pos)) buildable = false;

        if (buildable) selectedTile.color = new Color(0, 1, 0, 0.7f);
        else selectedTile.color = new Color(1, 0, 0, 0.7f);

        // ���õ� Ÿ���� ��ġ�� localPosition���� �������� ������ grid���� ��ġ�� ��߳�.
        selectedTile.transform.localPosition = pos;

        return buildable;
    }

    public Vector3 GetMapPos(Vector3 pos)
    {
        /* Ÿ���� grid.cellSize��ŭ �������� ����
         * ���� worldPos���� ������ ���� �簢���� ���� �Ʒ� �������� ����.
         * ���� cellSize�� ���ݸ�ŭ ��ġ�� �������ָ� Ÿ���� ��ġ�� ��.
         */
        float x = Mathf.FloorToInt(pos.x) + grid.cellSize.x / 2;
        float y = Mathf.FloorToInt(pos.y) + grid.cellSize.y / 2;

        return new Vector3(x, y);
    }
    #endregion
}
