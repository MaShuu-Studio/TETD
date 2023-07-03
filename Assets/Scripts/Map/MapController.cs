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

        grid = GetComponent<Grid>();

        transform.position = Vector3.zero;
        OffSeletedTile();
    }

    [SerializeField] private SpriteRenderer[] backgrounds;
    [SerializeField] private SpriteRenderer selectedTile;
    [SerializeField] private Tilemap tilemap;
    private Grid grid;
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

        Sprite[] sprites = map.tilemap.GetBackGround();
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (sprites[i] == null)
            {
                backgrounds[i].gameObject.SetActive(false);
                continue;
            }
            // ��ü����� 640*360�� �� �� �°� �Ǿ�����.
            // �̿� ���缭 ������ ����

            float x, y;
            x = sprites[i].texture.width;
            y = sprites[i].texture.height;

            backgrounds[i].transform.localScale = new Vector3(640 / x, 360 / y);
            backgrounds[i].sprite = sprites[i];
            backgrounds[i].gameObject.SetActive(true);
        }

        foreach (var pos in map.tilemap.tiles.Keys)
        {
            CustomRuleTile ruleTile = map.tilemap.GetTile(pos);
            if (ruleTile != null)
            {
                string[] info = new string[4] { "", "", "", "" };
                Vector3Int[] dir = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                for (int i = 0; i < 4; i++)
                {
                    CustomRuleTile aroundTile = map.tilemap.GetTile(pos + dir[i]);
                    if (aroundTile != null) info[i] = aroundTile.type;
                }
                CustomTile tile = ruleTile.GetTile(info);

                tilemap.SetTile(pos, tile);
            }
        }
    }
    #region Tile
    // Update is called once per frame
    private void Update()
    {
        if (CameraController.Instance.Cam == null) return;

        bool click = Input.GetMouseButtonDown(0);

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = CameraController.Instance.Cam.ScreenToWorldPoint(mousePos);
        Vector3 pos = GetMapPos(worldPos);

        if (readyToBuild)
        {
            bool buildable = SelectTile(worldPos);
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
    }

    public void ReadyToBuild(int id)
    {
        this.id = id;
        selectedTile.sprite = SpriteManager.GetSprite(id);
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
