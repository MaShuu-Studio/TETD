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

    public TilemapInfo Tilemap 
    { 
        get 
        {
            // Ÿ�ϸʿ� enemyRoad �ο�.
            tilemap.enemyRoad = enemyRoad;
            return tilemap; 
        }
    }
    private TilemapInfo tilemap;

    private List<Vector3Int> enemyRoad;
    private CustomRuleTile selectedTile;
    private bool drawFlag;

    // �������� �������� �־�� ���̺� ����.
    public bool CanSave { get { return enemyRoad.Count >= 2; } }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }

    #region Map Controller

    public void Init(Map map, string mapName)
    {
        SetActive(true);
        this.mapName = mapName;

        if (map == null) tilemap = new TilemapInfo();
        else tilemap = new TilemapInfo(map.tilemap);
        enemyRoad = new List<Vector3Int>();

        SetBackground(tilemap.backgroundName);

        UpdateMap();
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameObject.activeSelf == false) return;

        bool click = Input.GetMouseButton(0);
        bool rclick = Input.GetMouseButton(1);

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        SelectTile(worldPos);

        if (UIController.Instance.PointInMapEditPanel() == false)
        {
            if (click && selectedTile != null)
            {
                if (drawFlag) SetRoad(tilePos, selectedTile.name);
                else SetTile(tilePos, selectedTile);
            }

            if (rclick)
            {
                if (drawFlag) SetRoad(tilePos, selectedTile.name, false);
                else SetTile(tilePos, null);
            }
        }
    }

    public void SetBackground(string name)
    {
        Sprite[] sprites = TileManager.GetBackground(name);

        backgrounds[0].gameObject.SetActive(false);
        backgrounds[1].gameObject.SetActive(false);
        if (sprites != null)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (sprites[i] == null) continue;

                // ��ü����� 640*360�� �� �� �°� �Ǿ�����.
                // �̿� ���缭 ������ ����

                float x, y;
                x = sprites[i].texture.width;
                y = sprites[i].texture.height;

                backgrounds[i].transform.localScale = new Vector3(640 / x, 360 / y);
                backgrounds[i].sprite = sprites[i];
                backgrounds[i].gameObject.SetActive(true);
            }
        }

        tilemap.backgroundName = name;
    }

    public bool SelectTile(Vector3 worldPos)
    {
        selectedPosSprite.gameObject.SetActive(true);

        // ���� WorldPos�� GetTilePos�� �ѹ� �� �����ִ� ������ �۾��� �Ǽ��� �ּ�ȭ �ϱ� ������.
        // ���� ��ġ�� Ÿ���� ��ġ�� �ణ �ٸ�.
        Vector3 pos = GetMapPos(worldPos);
        Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        bool buildable = (map != null && map.tilemap.Buildable(tilePos));

        if (buildable) selectedPosSprite.color = new Color(0, 1, 0, 0.7f);
        else selectedPosSprite.color = new Color(1, 0, 0, 0.7f);

        // ���õ� Ÿ���� ��ġ�� localPosition���� �������� ������ grid���� ��ġ�� ��߳�.
        selectedPosSprite.transform.localPosition = pos;

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
    public void SelectTile(CustomRuleTile tile, bool isFlag)
    {
        selectedTile = tile;
        selectedPosSprite.sprite = tile.Base.sprite;
        drawFlag = isFlag;
    }

    #region Map
    // Flag�� �����ϴµ� �۵��ϴ� �ڵ�
    private void SetRoad(Vector3Int pos, string flagName, bool add = true)
    {
        // Flag�� Road���� ��ġ�� �� ����.
        // pos ĭ�� ����ְų� Road�� �ƴϸ� ���
        if (tilemap.tiles.ContainsKey(pos) == false || tilemap.GetTile(pos).type != "ROAD") return;

        // �߰�
        if (add)
        {
            // StartFlag�� DestFlag�� �̷����.
            // ���� ���� Ȥ�� ���������� ������ ���� �ʴٸ� �׷����� ����.
            // ������ �´ٸ� �� ���̸� �ڳʷ� �� �޿�� ���.
            // ������ ��ģ�ٸ� �� �κ��� �� ��������.
            if (flagName == "STARTFLAG")
            {
                // �켱 road�� �ִ��� üũ.
                if (enemyRoad.Count > 0)
                {
                    // �ִٸ� ���� �� �κа� ���� üũ.
                    Vector3Int targetPos = enemyRoad[0];
                    Vector3Int dir = targetPos - pos;

                    // ������ ���� �ʴٸ� ��ġ���� ����.
                    // Ȥ�� ���� ���� ��ġ�ϴ��� �ǹ� ������ ���.
                    if ((dir.x != 0 && dir.y != 0)
                        || (dir.x == 0 && dir.y == 0)) return;

                    // dir�� normarlize��.
                    if (dir.x > 0) dir.x = 1;
                    else if (dir.x < 0) dir.x = -1;

                    if (dir.y > 0) dir.y = 1;
                    else if (dir.y < 0) dir.y = -1;

                    // �� ���� ���� ������ ������� �ִ� Road���� üũ.
                    // ������ ������� �ִٸ� �� �κ��� ���� ����.
                    int index = enemyRoad.FindIndex(p => p == pos);
                    if (index < 0) // ���ٸ� ���⿡ ���缭 ���̸� �޿��ִ� ���.
                    {
                        List<Vector3Int> startRoad = new List<Vector3Int>();
                        startRoad.Add(pos); // ���� �� �κ����ν� �߰�.
                        Vector3Int nextPos = pos + dir;
                        while (nextPos != targetPos)
                        {
                            startRoad.Add(nextPos);
                            nextPos += dir;
                            // ���࿡ �ش� ���⿡ Road�� ���ٸ� �� ���� ���� �� ���� ����.
                            if (tilemap.tiles.ContainsKey(nextPos) == false || tilemap.GetTile(nextPos).type != "ROAD")
                                return;
                        }

                        startRoad.AddRange(enemyRoad);
                        enemyRoad = startRoad;

                    }
                    else // �ִٸ� �ش� �κ� �� �κ��� ���� ����.
                    {
                        enemyRoad.RemoveRange(0, index);
                    }
                }
                else
                {
                    // road�� ���ٸ� �ڿ������� �߰�.
                    enemyRoad.Add(pos);
                }
            }
            else if (flagName == "DESTFLAG")
            {
                // �켱 road�� �ִ��� üũ.
                if (enemyRoad.Count > 0)
                {
                    // �ִٸ� ���� �� �κа� ���� üũ.
                    Vector3Int targetPos = enemyRoad[enemyRoad.Count - 1];
                    Vector3Int dir = pos - targetPos;

                    // ������ ���� �ʴٸ� ��ġ���� ����.
                    // Ȥ�� ���� ���� ��ġ�ϴ��� �ǹ� ������ ���.
                    if ((dir.x != 0 && dir.y != 0)
                        || (dir.x == 0 && dir.y == 0)) return;

                    // dir�� normarlize��.
                    if (dir.x > 0) dir.x = 1;
                    else if (dir.x < 0) dir.x = -1;

                    if (dir.y > 0) dir.y = 1;
                    else if (dir.y < 0) dir.y = -1;

                    // �� ���� ���� ������ ������� �ִ� Road���� üũ.
                    // ������ ������� �ִٸ� �� �κ��� ���� ����.
                    int index = enemyRoad.FindIndex(p => p == pos);
                    if (index < 0) // ���ٸ� ���⿡ ���缭 ���̸� �޿��ִ� ���.
                    {
                        List<Vector3Int> destRoad = new List<Vector3Int>();
                        Vector3Int nextPos = targetPos;
                        while (nextPos != pos)
                        {
                            nextPos += dir;
                            if (tilemap.tiles.ContainsKey(nextPos) == false || tilemap.GetTile(nextPos).type != "ROAD")
                                return;
                            destRoad.Add(nextPos);
                        }

                        enemyRoad.AddRange(destRoad);

                    }
                    else // �ִٸ� �ش� �κ� �� �κ��� ���� ����.
                    {
                        enemyRoad.RemoveRange(index + 1, enemyRoad.Count - 1 - index);
                    }
                }
                else
                {
                    // road�� ���ٸ� �ڿ������� �߰�.
                    enemyRoad.Add(pos);
                }
            }
        }
        // ����
        else
        {
            // STARTFLAG�� ���� ���ʱ��� ����(LastIndex)
            // DESTFLAG�� ���� ���ʱ��� ����(Index)
            if (flagName == "STARTFLAG")
            {
                int index = enemyRoad.FindIndex(p => p == pos);
                if (index < 0) return;
                enemyRoad.RemoveRange(0, index + 1);
            }
            else if (flagName == "DESTFLAG")
            {
                int index = enemyRoad.FindLastIndex(p => p == pos);
                if (index < 0) return;
                enemyRoad.RemoveRange(index, enemyRoad.Count - index);
            }
        }

        UpdateRoad();
    }

    private void SetTile(Vector3Int pos, CustomRuleTile tile)
    {
        bool update = true;
        if (tilemap.tiles.ContainsKey(pos))
        {
            if (tile == null)
            {
                if (tilemap.GetTile(pos).type == "ROAD" && enemyRoad.Contains(pos))
                {
                    enemyRoad.Clear();
                    UpdateRoad();
                }
                tilemap.tiles.Remove(pos);
            }

            else if (tilemap.tiles[pos].name != tile.name)
            {
                if ((tilemap.GetTile(pos).type == "ROAD" && tile.type != "ROAD")
                    && enemyRoad.Contains(pos))
                {
                    enemyRoad.Clear();
                    UpdateRoad();
                }
                tilemap.tiles[pos] = new TileInfo(tile.name, tile.Base.buildable);
            }
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
    public void Clear()
    {
        mapTilemap.ClearAllTiles();
        buildableTilemap.ClearAllTiles();
        routeTilemap.ClearAllTiles();
    }

    private void UpdateMap(Vector3Int selectedPos)
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
                    if (aroundTile != null) info[j] = aroundTile.type;
                }

                tile = ruleTile.GetTile(info);

                if (drawFlag == false)
                    buildableFlag = (tile.buildable) ? TileManager.GetFlag("BUILDABLE").Base : TileManager.GetFlag("NOTBUILDABLE").Base;
            }

            mapTilemap.SetTile(pos, tile);
            if (drawFlag == false) 
                buildableTilemap.SetTile(pos, buildableFlag);
        }
    }

    private void UpdateRoad()
    {
        routeTilemap.ClearAllTiles();
        if (enemyRoad.Count == 0) return;

        // ���� ù �κ��� STARTFLAG, ������ �κ��� DESTFLAG��.
        int index = 0;
        routeTilemap.SetTile(enemyRoad[index], TileManager.GetFlag("STARTFLAG").Base);
        for (index = 1; index < enemyRoad.Count - 1; index++)
        {
            Vector3Int pos = enemyRoad[index];
            Vector3Int[] dir = new Vector3Int[5] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.zero };

            CustomRuleTile cornerTile = TileManager.GetTile("CORNER");
            string[] info = new string[4] { "", "", "", "" };
            for (int i = 0; i < dir.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (enemyRoad.Contains(pos + dir[j])) info[j] = "FLAG";
                }
                CustomTile tile = cornerTile.GetTile(info);
                routeTilemap.SetTile(pos, tile);
            }
        }
        if (index < enemyRoad.Count)
            routeTilemap.SetTile(enemyRoad[index], TileManager.GetFlag("DESTFLAG").Base);
    }

    // ��ü ���� ������Ʈ
    public void UpdateMap()
    {
        Clear();

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
                    if (aroundTile != null) info[i] = aroundTile.type;
                }

                CustomTile tile = ruleTile.GetTile(info);
                bool b = tile.buildable;

                mapTilemap.SetTile(pos, tile);
                buildableTilemap.SetTile(pos, (b) ? TileManager.GetFlag("BUILDABLE").Base : TileManager.GetFlag("NOTBUILDABLE").Base);
            }
        }

        if (tilemap.enemyRoad.Count > 0)
        {
            // ������ ����.
            CustomRuleTile cornerTile = TileManager.GetTile("CORNER");
            CustomRuleTile ruleTile = TileManager.GetTile("STARTFLAG");
            routeTilemap.SetTile(tilemap.enemyRoad[0], ruleTile.Base);
            enemyRoad.Add(tilemap.enemyRoad[0]);

            for (int i = 1; i < tilemap.enemyRoad.Count; i++)
            {
                Vector3Int start = tilemap.enemyRoad[i - 1];
                Vector3Int dest = tilemap.enemyRoad[i];
                Vector3Int d = dest - start;

                if (d.x > 0) d.x = 1;
                else if (d.x < 0) d.x = -1;

                if (d.y > 0) d.y = 1;
                else if (d.y < 0) d.y = -1;

                Vector3Int pos = start + d;

                ruleTile = TileManager.GetTile("CORNER");

                if (i == tilemap.enemyRoad.Count - 1)
                    ruleTile = TileManager.GetTile("DESTFLAG");

                // �߰� �κ��� corner�� ä��� ����.
                CustomTile tile;
                while (pos != dest)
                {
                    string[] info = new string[4] { "", "", "", "" };
                    Vector3Int[] dir = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                    // ���� ��ġ�� �ӽ� ��ġ. ���� ��ġ�� �������̴��� ���ġ�� �̷���� ���̱� ������ ��� X
                    routeTilemap.SetTile(pos + d, cornerTile.Base);
                    enemyRoad.Add(pos + d);

                    // �� �� ���� Ÿ�Ͽ� ���� Rule Ȯ��.
                    for (int j = 0; j < 4; j++)
                    {
                        if (enemyRoad.Contains(pos + dir[j])) info[j] = "FLAG";
                    }
                    tile = cornerTile.GetTile(info);
                    routeTilemap.SetTile(pos, tile);

                    enemyRoad.Add(pos);
                    pos += d;
                }

                // ������ ���̰ų� ������ �κ��̱� ������ Base�� �����ص� ��.
                routeTilemap.SetTile(dest, ruleTile.Base);
                enemyRoad.Add(dest);
            }
        }
    }
    #endregion
}
