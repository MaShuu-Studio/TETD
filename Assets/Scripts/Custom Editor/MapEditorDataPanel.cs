using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

public class MapEditorDataPanel : MonoBehaviour
{
    [SerializeField] private Transform[] toggles;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject[] addButtons;

    [Header("MAP PANEL")]
    [SerializeField] private MapEditorMapProperty mapPropertyPrefab;
    [SerializeField] private Transform mapPanelParent;
    private List<MapProperty> mapProperties;
    private List<MapEditorMapProperty> mapPropertyIcons;

    [Header("TILE PANEL")]
    [SerializeField] private MapEditorTileProperty tilePropertyPrefab;
    [SerializeField] private Transform tilePanelParent;
    private Dictionary<Vector3Int, TileProperty> tileProperties;
    private Dictionary<Vector3Int, MapEditorTileProperty> tilePropertyIcons;

    [Header("ROUND PANEL")]
    [SerializeField] private MapEditorRoundInfo roundInfoPrefab;
    [SerializeField] private Transform roundPanelParent;

    private int selectedPanel;

    public void Init()
    {
        mapPropertyPrefab.gameObject.SetActive(false);
        tilePropertyPrefab.gameObject.SetActive(false);
        mapPropertyIcons = new List<MapEditorMapProperty>();

        tilePropertyIcons = new Dictionary<Vector3Int, MapEditorTileProperty>();
    }

    public void LoadMap()
    {
        Map map = MapEditor.Instance.MapData;

        selectedPanel = 0;
        LoadPanel(selectedPanel);

        foreach (var icon in mapPropertyIcons)
        {
            Destroy(icon.gameObject);
        }
        mapPropertyIcons.Clear();
        foreach (var icon in tilePropertyIcons.Values)
        {
            Destroy(icon.gameObject);
        }
        tilePropertyIcons.Clear();

        mapProperties = map.mapProperties;
        foreach (var prop in mapProperties)
        {
            MapEditorMapProperty propIcon = Instantiate(mapPropertyPrefab);
            propIcon.transform.SetParent(mapPanelParent);
            propIcon.transform.localScale = Vector3.one;
            propIcon.gameObject.SetActive(true);
            propIcon.SetProp(mapPropertyIcons.Count, prop, this);

            mapPropertyIcons.Add(propIcon);
        }
        UpdateElements();
        if (GetRemainElements().Count == 0) addButtons[0].gameObject.SetActive(false);
        else addButtons[0].transform.SetAsLastSibling();

        tileProperties = map.tileProperties;
        foreach (var prop in tileProperties)
        {
            MapEditorTileProperty propIcon = Instantiate(tilePropertyPrefab);
            propIcon.transform.SetParent(tilePanelParent);
            propIcon.transform.localScale = Vector3.one;
            propIcon.gameObject.SetActive(true);
            tilePropertyIcons.Add(prop.Key, propIcon);

            propIcon.SetProp(tilePropertyIcons.Count, prop.Key, prop.Value, this);
            UIController.Instance.AddSpecialTile(prop.Key);
        }
    }

    public List<Element> GetRemainElements()
    {
        List<Element> elements = new List<Element>();
        for (int i = 0; i < EnumArray.Elements.Length; i++)
        {
            bool add = true;
            Element e = EnumArray.Elements[i];
            foreach (var prop in mapProperties)
            {
                // 목록을 쭉 훑어서 같은 속성이 있다면 추가하지 않음.
                if (e == prop.element)
                {
                    add = false;
                    break;
                }
            }

            // 선택된 속성이 목록에 없다면 추가.
            if (add) elements.Add(e);
        }

        return elements;
    }

    public void UpdateElements()
    {
        for (int i = 0; i < mapPropertyIcons.Count; i++)
        {
            mapPropertyIcons[i].UpdateElement(i, GetRemainElements());
        }
    }
    public void AddData()
    {
        // Map Property
        if (selectedPanel == 0)
        {
            List<Element> elements = GetRemainElements();
            if (elements.Count > 0)
            {
                MapProperty mp = new MapProperty();
                mp.element = elements[0];

                MapEditorMapProperty propIcon = Instantiate(mapPropertyPrefab);
                propIcon.transform.SetParent(mapPanelParent);
                propIcon.transform.localScale = Vector3.one;
                propIcon.gameObject.SetActive(true);
                propIcon.SetProp(mapPropertyIcons.Count, mp, this);

                mapProperties.Add(mp);
                mapPropertyIcons.Add(propIcon);
                UpdateElements();

                addButtons[selectedPanel].transform.SetAsLastSibling();
                // 만약에 마지막 속성이라면 addButton을 비활성화함.
                if (elements.Count == 1)
                    addButtons[selectedPanel].SetActive(false);
            }
        }
        // Round
        else if (selectedPanel == 2)
        {

        }
    }

    public void RemoveData(int index)
    {
        if (selectedPanel == 0)
        {
            mapProperties.RemoveAt(index);
            Destroy(mapPropertyIcons[index].gameObject);
            mapPropertyIcons.RemoveAt(index);

            UpdateElements();

            addButtons[0].SetActive(true);
            addButtons[0].transform.SetAsLastSibling();
        }
    }

    public void AddData(Vector3Int pos, int number)
    {
        if (tileProperties.ContainsKey(pos)) return;

        TileProperty prop = new TileProperty();

        MapEditorTileProperty propIcon = Instantiate(tilePropertyPrefab);
        propIcon.transform.SetParent(tilePanelParent);
        propIcon.transform.localScale = Vector3.one;
        propIcon.gameObject.SetActive(true);
        propIcon.SetProp(number, pos, prop, this);

        tileProperties.Add(pos, prop);
        tilePropertyIcons.Add(pos, propIcon);
    }

    public void RemoveData(Vector3Int pos)
    {
        UIController.Instance.RemoveNumbering(pos);
        tileProperties.Remove(pos);
        Destroy(tilePropertyIcons[pos].gameObject);
        tilePropertyIcons.Remove(pos);
    }

    public void UpdateNumber(Vector3Int pos, int number)
    {
        tilePropertyIcons[pos].UpdateNumber(number);
    }

    public void LoadPanel(int index)
    {
        selectedPanel = index;
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index) toggles[i].SetAsLastSibling();
            else toggles[i].SetAsFirstSibling();

            panels[i].SetActive(i == index);
        }
    }
}
