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

    // �ӽ� ����
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

        // ���õ� Ÿ���� ��ġ�� localPosition���� �������� ������ grid���� ��ġ�� ��߳�.
        selectedTile.transform.localPosition = GetTilePos(worldPos);
    }

    public Vector2 GetTilePos(Vector2 pos)
    {
        /* Ÿ���� grid.cellSize��ŭ �������� ����
         * ���� worldPos���� ������ ���� �簢���� ���� �Ʒ� �������� ����.
         * ���� cellSize�� ���ݸ�ŭ ��ġ�� �������ָ� Ÿ���� ��ġ�� ��.
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
