using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoIcon : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private Transform tilesParent;
    [SerializeField] private Image[] backgrounds;
    [SerializeField] private Image tileBase;
    private RectTransform rect;
    private Image[,] mapTiles;

    [Space]
    [Header("Info")]
    [SerializeField] private RectTransform summarizedInfosParent;
    [SerializeField] private MapSummarizedInfo summarizedInfoPrefab;
    [Header("More Info")]
    [SerializeField] private GameObject moreInfoIcon;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform moreInfoParent;
    [SerializeField] private MapMoreInfo moreInfoPrefab;
    private List<GameObject> infoObjectList = new List<GameObject>();

    public Map MapData { get { return map; } }
    private Map map;
    private float tileSize;

    private void Update()
    {
        if (moreInfoIcon.activeSelf)
            background.gameObject.SetActive(UIController.PointOverUI(moreInfoIcon));
    }

    public void SetIcon(Map map)
    {
        rect = GetComponent<RectTransform>();
        tileSize = rect.sizeDelta.y / 15;
        RectTransform tileRect = tileBase.rectTransform;
        tileRect.sizeDelta = Vector2.one * tileSize;
        transform.localScale = Vector3.one;

        summarizedInfoPrefab.gameObject.SetActive(false);
        moreInfoPrefab.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        foreach (var go in infoObjectList)
        {
            Destroy(go);
        }
        infoObjectList.Clear();

        this.map = map;
        if (this.map.mapProperties != null)
        {
            foreach (var prop in this.map.mapProperties)
            {
                // Summarized Info
                MapSummarizedInfo.UpDown updown = MapSummarizedInfo.UpDown.MID;
                if (prop.atk > 0) updown--;
                else if (prop.atk < 0) updown++;
                if (prop.atkSpeed > 0) updown--;
                else if (prop.atkSpeed < 0) updown++;

                if (updown < 0) updown = MapSummarizedInfo.UpDown.UP;
                else if ((int)updown > 2) updown = MapSummarizedInfo.UpDown.DOWN;

                MapSummarizedInfo info = Instantiate(summarizedInfoPrefab);
                info.gameObject.SetActive(true);
                info.SetInfo(prop.element, updown);
                info.transform.SetParent(summarizedInfosParent);
                info.transform.localScale = Vector3.one;
                infoObjectList.Add(info.gameObject);

                // More Info
                MapMoreInfo moreInfo = Instantiate(moreInfoPrefab);
                moreInfo.gameObject.SetActive(true);
                moreInfo.SetInfo(prop);
                moreInfo.transform.SetParent(moreInfoParent);
                moreInfo.transform.localScale = Vector3.one;
                infoObjectList.Add(moreInfo.gameObject);
            }
            summarizedInfosParent.sizeDelta = new Vector2(96 * this.map.mapProperties.Count, 48);
            background.sizeDelta = new Vector2(264, 72 + this.map.mapProperties.Count * 48);
        }

        UpdateMap(map);
    }

    private void UpdateMap(Map map)
    {
        Sprite[] backgrounds = map.GetBackGround();
        if (backgrounds == null)
        {
            for (int i = 0; i < this.backgrounds.Length; i++)
                this.backgrounds[i].gameObject.SetActive(false);
        }
        else
            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (backgrounds[i] == null) this.backgrounds[i].gameObject.SetActive(false);
                else
                {
                    this.backgrounds[i].gameObject.SetActive(true);
                    this.backgrounds[i].sprite = backgrounds[i];
                }
            }

        if (mapTiles != null)
        {
            foreach (var tile in mapTiles)
                Destroy(tile.gameObject);
        }
        mapTiles = new Image[map.size.y, map.size.x];
        tileBase.gameObject.SetActive(false);

        for (int y = map.size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(map.origin.x + x, map.origin.y + y);
                Image tile = Instantiate(tileBase);
                CustomTile data = null;

                CustomRuleTile ruleTile = map.GetTile(pos);
                if (ruleTile != null)
                {
                    string[] info = new string[4] { "", "", "", "" };
                    Vector3Int[] dir = new Vector3Int[4] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                    for (int i = 0; i < 4; i++)
                    {
                        CustomRuleTile aroundTile = map.GetTile(pos + dir[i]);
                        if (aroundTile != null) info[i] = aroundTile.type;
                    }
                    data = ruleTile.GetTile(info);
                }
                else
                {
                    tile.color = new Color(0, 0, 0, 0);
                }
                tile.transform.SetParent(tilesParent);
                tile.transform.localScale = Vector3.one;
                ((RectTransform)(tile.transform)).anchoredPosition = new Vector3(pos.x, pos.y) * tileSize;
                tile.sprite = (data != null) ? data.sprite : null;
                tile.gameObject.SetActive(true);

                mapTiles[y, x] = tile;
            }
        }
    }
}
