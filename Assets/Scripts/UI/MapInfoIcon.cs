using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoIcon : MonoBehaviour
{
    [SerializeField] private Transform tilesParent;
    [SerializeField] private Image[] backgrounds;
    [SerializeField] private Image tileBase;
    private RectTransform rect;
    private Image[,] mapTiles;
    public Map MapData { get { return map; } }
    private Map map;
    private float tileSize;
    public void SetIcon(Map map)
    {
        rect = GetComponent<RectTransform>();
        tileSize = rect.sizeDelta.y / 15;
        RectTransform tileRect = tileBase.rectTransform;
        tileRect.sizeDelta = Vector2.one * tileSize;
        transform.localScale = Vector3.one;

        this.map = map;

        UpdateMap(map);
    }

    private void UpdateMap(Map map)
    {
        Sprite[] backgrounds = map.tilemap.GetBackGround();
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
        mapTiles = new Image[map.tilemap.size.y, map.tilemap.size.x];
        tileBase.gameObject.SetActive(false);

        for (int y = map.tilemap.size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.tilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(map.tilemap.origin.x + x, map.tilemap.origin.y + y);
                Image tile = Instantiate(tileBase);
                CustomTile data = null;

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
