using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using EnumData;
using TMPro;

public class GameSettingController : MonoBehaviour
{
    [SerializeField] private List<GameSettingIcon> charIcons;
    [SerializeField] private List<GameSettingIcon> difficultIcons;
    [SerializeField] private Transform elementParent;
    [SerializeField] private GameSettingIcon elementIconPrefab;
    [SerializeField] private TextMeshProUGUI infoText;
    private List<GameSettingIcon> elementIcons;

    [Header("Map Setting")]
    [SerializeField] private GameObject mapListObject;
    [SerializeField] private MapInfoIcon mapIconPrefab;
    [SerializeField] private SelectMapButton selectMapButtonPrefab;
    [SerializeField] private MapInfoIcon currentMap;
    [SerializeField] private ScrollRect mapList;
    [SerializeField] private RectTransform mapButtonsRect;
    [SerializeField] private Transform mapParent;
    private List<MapInfoIcon> mapIcons;
    private int selectedMap;

    public async Task Init()
    {
        while (SpriteManager.isLoad == false) await Task.Yield();

        // 추후 해당 부분을 데이터화 및 자동 생성시켜 작동시킬 예정
        for (int i = 0; i < charIcons.Count; i++)
        {
            charIcons[i].SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.CHARTYPE, i), ((CharacterType)i).ToString());
            charIcons[i].isOn = false;
        }

        for (int i = 0; i < difficultIcons.Count; i++)
        {
            difficultIcons[i].SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.DIFF, i), ((DifficultyType)i).ToString());
            difficultIcons[i].isOn = false;
        }

        elementIcons = new List<GameSettingIcon>();
        for (int i = 0; i < EnumArray.Elements.Length; i++)
        {
            Element e = (Element)i;
            GameSettingIcon icon = Instantiate(elementIconPrefab);
            icon.SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, i), EnumArray.ElementStrings[e]);
            icon.transform.SetParent(elementParent);
            icon.transform.localScale = Vector3.one;
            icon.isOn = false;
            elementIcons.Add(icon);
        }

        elementIconPrefab.gameObject.SetActive(false);

        if (mapIcons != null)
        {
            foreach (var icon in mapIcons)
            {
                Destroy(icon.gameObject);
            }
            mapIcons.Clear();
        }
        mapIcons = new List<MapInfoIcon>();

        while (MapManager.Maps == null) await Task.Yield();

        mapListObject.SetActive(false);
        selectMapButtonPrefab.gameObject.SetActive(false);
        mapIconPrefab.gameObject.SetActive(false);
        int size = 0;
        for (int i = 0; i < MapManager.Maps.Count; i++)
        {
            size += 110;

            string mapName = MapManager.Maps[i];
            Map map = await MapManager.LoadMap(mapName);

            MapInfoIcon mapIcon = Instantiate(mapIconPrefab);
            mapIcon.SetIcon(map);
            mapIcon.transform.SetParent(mapParent);
            mapIcon.transform.localScale = Vector3.one;
            ((RectTransform)(mapIcon.transform)).anchoredPosition = Vector3.zero;
            mapIcon.gameObject.SetActive(false);

            SelectMapButton mapButton = Instantiate(selectMapButtonPrefab);
            mapButton.Init(mapName, this, i);
            mapButton.transform.SetParent(mapButtonsRect.transform);
            mapButton.transform.localScale = Vector3.one;
            mapButton.gameObject.SetActive(true);

            mapIcons.Add(mapIcon);
        }

        charIcons[0].isOn = true;
        SelectMap(0);

        mapButtonsRect.sizeDelta = new Vector2(300, size);
    }

    public void SelectMap(int index)
    {
        currentMap.SetIcon(mapIcons[index].MapData);
        mapListObject.SetActive(false);
    }

    public void ShowMap(int index)
    {
        mapIcons[selectedMap].gameObject.SetActive(false);
        mapIcons[index].gameObject.SetActive(true);
        selectedMap = index;
    }

    public void ShowInfo(bool b, string str)
    {
        if (b == false) return;

        infoText.text = str;
    }

    public CharacterType SelectedCharacter()
    {
        for (int i = 0; i < charIcons.Count; i++)
        {
            if (charIcons[i].isOn) return (CharacterType)i;
        }
        return (CharacterType)0;
    }

    public List<DifficultyType> Difficulty()
    {
        List<DifficultyType> list = new List<DifficultyType>();

        for (int i = 0; i < difficultIcons.Count; i++)
        {
            if (difficultIcons[i].isOn) list.Add((DifficultyType)i);
        }

        return list;
    }

    public string MapName()
    {
        return MapManager.Maps[selectedMap];
    }
}
