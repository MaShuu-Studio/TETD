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
            // 타일맵에 enemyRoad 부여.
            tilemap.enemyRoad = enemyRoad;
            return tilemap; 
        }
    }
    private TilemapInfo tilemap;

    private List<Vector3Int> enemyRoad;
    private CustomRuleTile selectedTile;
    private bool drawFlag;

    // 시작점과 도착점은 있어야 세이브 가능.
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

        tilemap.backgroundName = name;
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
    // Flag를 세팅하는데 작동하는 코드
    private void SetRoad(Vector3Int pos, string flagName, bool add = true)
    {
        // Flag는 Road에만 설치할 수 있음.
        // pos 칸이 비어있거나 Road가 아니면 취소
        if (tilemap.tiles.ContainsKey(pos) == false || tilemap.GetTile(pos).type != "ROAD") return;

        // 추가
        if (add)
        {
            // StartFlag와 DestFlag로 이루어짐.
            // 기존 시작 혹은 도착지점과 방향이 맞지 않다면 그려지지 않음.
            // 방향이 맞다면 그 사이를 코너로 다 메우는 방식.
            // 방향이 겹친다면 앞 부분을 다 날려버림.
            if (flagName == "STARTFLAG")
            {
                // 우선 road가 있는지 체크.
                if (enemyRoad.Count > 0)
                {
                    // 있다면 가장 앞 부분과 방향 체크.
                    Vector3Int targetPos = enemyRoad[0];
                    Vector3Int dir = targetPos - pos;

                    // 방향이 맞지 않다면 설치하지 않음.
                    // 혹은 같은 곳에 설치하더라도 의미 없으니 취소.
                    if ((dir.x != 0 && dir.y != 0)
                        || (dir.x == 0 && dir.y == 0)) return;

                    // dir를 normarlize함.
                    if (dir.x > 0) dir.x = 1;
                    else if (dir.x < 0) dir.x = -1;

                    if (dir.y > 0) dir.y = 1;
                    else if (dir.y < 0) dir.y = -1;

                    // 그 외의 경우는 기존에 만들어져 있는 Road인지 체크.
                    // 기존에 만들어져 있다면 앞 부분을 전부 날림.
                    int index = enemyRoad.FindIndex(p => p == pos);
                    if (index < 0) // 없다면 방향에 맞춰서 사이를 메워주는 방식.
                    {
                        List<Vector3Int> startRoad = new List<Vector3Int>();
                        startRoad.Add(pos); // 가장 앞 부분으로써 추가.
                        Vector3Int nextPos = pos + dir;
                        while (nextPos != targetPos)
                        {
                            startRoad.Add(nextPos);
                            nextPos += dir;
                            // 만약에 해당 방향에 Road가 없다면 이 역시 지을 수 없는 형태.
                            if (tilemap.tiles.ContainsKey(nextPos) == false || tilemap.GetTile(nextPos).type != "ROAD")
                                return;
                        }

                        startRoad.AddRange(enemyRoad);
                        enemyRoad = startRoad;

                    }
                    else // 있다면 해당 부분 앞 부분을 전부 없앰.
                    {
                        enemyRoad.RemoveRange(0, index);
                    }
                }
                else
                {
                    // road가 없다면 자연스럽게 추가.
                    enemyRoad.Add(pos);
                }
            }
            else if (flagName == "DESTFLAG")
            {
                // 우선 road가 있는지 체크.
                if (enemyRoad.Count > 0)
                {
                    // 있다면 가장 뒷 부분과 방향 체크.
                    Vector3Int targetPos = enemyRoad[enemyRoad.Count - 1];
                    Vector3Int dir = pos - targetPos;

                    // 방향이 맞지 않다면 설치하지 않음.
                    // 혹은 같은 곳에 설치하더라도 의미 없으니 취소.
                    if ((dir.x != 0 && dir.y != 0)
                        || (dir.x == 0 && dir.y == 0)) return;

                    // dir를 normarlize함.
                    if (dir.x > 0) dir.x = 1;
                    else if (dir.x < 0) dir.x = -1;

                    if (dir.y > 0) dir.y = 1;
                    else if (dir.y < 0) dir.y = -1;

                    // 그 외의 경우는 기존에 만들어져 있는 Road인지 체크.
                    // 기존에 만들어져 있다면 뒷 부분을 전부 날림.
                    int index = enemyRoad.FindIndex(p => p == pos);
                    if (index < 0) // 없다면 방향에 맞춰서 사이를 메워주는 방식.
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
                    else // 있다면 해당 부분 앞 부분을 전부 없앰.
                    {
                        enemyRoad.RemoveRange(index + 1, enemyRoad.Count - 1 - index);
                    }
                }
                else
                {
                    // road가 없다면 자연스럽게 추가.
                    enemyRoad.Add(pos);
                }
            }
        }
        // 삭제
        else
        {
            // STARTFLAG면 가장 뒤쪽까지 삭제(LastIndex)
            // DESTFLAG면 가장 앞쪽까지 삭제(Index)
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

        // 가장 첫 부분은 STARTFLAG, 마지막 부분은 DESTFLAG임.
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

    // 전체 맵을 업데이트
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
            // 시작점 세팅.
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

                // 중간 부분을 corner로 채우는 과정.
                CustomTile tile;
                while (pos != dest)
                {
                    string[] info = new string[4] { "", "", "", "" };
                    Vector3Int[] dir = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                    // 다음 위치에 임시 배치. 다음 위치가 마지막이더라도 재배치가 이루어질 것이기 때문에 상관 X
                    routeTilemap.SetTile(pos + d, cornerTile.Base);
                    enemyRoad.Add(pos + d);

                    // 이 후 현재 타일에 대해 Rule 확인.
                    for (int j = 0; j < 4; j++)
                    {
                        if (enemyRoad.Contains(pos + dir[j])) info[j] = "FLAG";
                    }
                    tile = cornerTile.GetTile(info);
                    routeTilemap.SetTile(pos, tile);

                    enemyRoad.Add(pos);
                    pos += d;
                }

                // 어차피 꺾이거나 끝나는 부분이기 때문에 Base로 지정해도 됨.
                routeTilemap.SetTile(dest, ruleTile.Base);
                enemyRoad.Add(dest);
            }
        }
    }
    #endregion
}
