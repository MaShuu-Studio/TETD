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
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
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
    private List<SelectMapButton> mapButtons;
    private int selectedMap;

    public async Task Init()
    {
        ShowInfo("", "");
        while (SpriteManager.isLoad == false) await Task.Yield();

        for (int i = 0; i < charIcons.Count; i++)
        {
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.CHARTYPE + i);
            charIcons[i].SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.CHARTYPE, i), lang);
            charIcons[i].isOn = false;
        }

        for (int i = 0; i < difficultIcons.Count; i++)
        {
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.DIFF + i);
            difficultIcons[i].SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.DIFF, i), lang);
            difficultIcons[i].isOn = false;
        }

        elementIcons = new List<GameSettingIcon>();
        for (int i = 0; i < EnumArray.Elements.Length; i++)
        {
            Element e = (Element)i;
            GameSettingIcon icon = Instantiate(elementIconPrefab);
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.ELEMENT + i);

            icon.SetIcon(this, SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, i), lang);
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
        mapButtons = new List<SelectMapButton>();

        while (MapManager.Maps == null) await Task.Yield();

        mapListObject.SetActive(false);
        selectMapButtonPrefab.gameObject.SetActive(false);
        mapIconPrefab.gameObject.SetActive(false);
        int size = 0;
        for (int i = 0; i < MapManager.Maps.Count; i++)
        {
            size += 110;

            string mapName = MapManager.Keys[i];
            Map map = MapManager.LoadMap(mapName);

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
            mapButtons.Add(mapButton);
        }

        charIcons[0].isOn = true;
        SelectMap(0);

        mapButtonsRect.sizeDelta = new Vector2(300, size);
    }

    public async Task UpdateMaps()
    {
        foreach(var icon in mapIcons)
        {
            Destroy(icon.gameObject);
        }
        mapIcons.Clear();

        foreach (var button in mapButtons)
        {
            Destroy(button.gameObject);
        }
        mapButtons.Clear();

        mapListObject.SetActive(false);
        selectMapButtonPrefab.gameObject.SetActive(false);
        mapIconPrefab.gameObject.SetActive(false);
        int size = 0;
        for (int i = 0; i < MapManager.Maps.Count; i++)
        {
            size += 110;

            string mapName = MapManager.Keys[i];
            Map map = MapManager.LoadMap(mapName);

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
            mapButtons.Add(mapButton);
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

    public void ShowInfo(string name, string desc)
    {
        nameText.text = name;
        descText.text = desc;
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
        return MapManager.Keys[selectedMap];
    }

    public void UpdateLanage()
    {
        for (int i = 0; i < charIcons.Count; i++)
        {
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.CHARTYPE + i);
            charIcons[i].UpdateLanguage(lang);
        }

        for (int i = 0; i < difficultIcons.Count; i++)
        {
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.DIFF + i);
            difficultIcons[i].UpdateLanguage(lang);
        }

        elementIcons = new List<GameSettingIcon>();
        for (int i = 0; i < elementIcons.Count; i++)
        {
            Language lang = Translator.GetLanguage((int)SpriteManager.ETCDataNumber.ELEMENT + i);
            elementIcons[i].UpdateLanguage(lang);
        }
        ShowInfo("", "");
    }
}
