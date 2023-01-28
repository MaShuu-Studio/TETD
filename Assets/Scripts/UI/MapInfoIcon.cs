using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoIcon : GameSettingIcon
{
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private Image tileBase;
    private RectTransform rect;
    private Image[,] mapTiles;
    public void SetIcon(Map map, ToggleGroup group)
    {
        desc.text = map.name;
        toggle.group = group;

        rect = GetComponent<RectTransform>();

        UpdateMap(map);
    }

    private void UpdateMap(Map map)
    {
        if (mapTiles != null)
        {
            foreach (var tile in mapTiles)
                Destroy(tile.gameObject);
        }
        mapTiles = new Image[map.tilemap.size.y, map.tilemap.size.x];
        grid.cellSize = new Vector2(rect.sizeDelta.x / map.tilemap.size.x, rect.sizeDelta.y / map.tilemap.size.y);
        tileBase.gameObject.SetActive(false);
        for (int y = map.tilemap.size.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.tilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(map.tilemap.origin.x + x, map.tilemap.origin.y + y);
                Image tile = Instantiate(tileBase);
                CustomTile data = map.tilemap.GetTile(pos);
                tile.transform.SetParent(grid.transform);
                tile.sprite = (data != null) ? data.sprite : null;
                tile.gameObject.SetActive(true);

                mapTiles[y, x] = tile;
            }
        }
    }

    public override void ChangeColor(bool b)
    {
        Color c = Color.white;
        if (b == false) c = Color.gray * 0.5f;

        image.color = c;
        for (int i = 0; i < mapTiles.GetLength(0); i++)
        {
            for (int j = 0; j < mapTiles.GetLength(1); j++)
            {
                mapTiles[i, j].color = c;
            }
        }
    }
}
