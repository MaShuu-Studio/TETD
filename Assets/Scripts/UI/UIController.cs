using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;
using System.Threading.Tasks;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get { return instance; } }
    private static UIController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        OpenSetting(false);
    }

    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private List<GameObject> scenes;

    [Space]
    [Header("Loading Scene")]
    [SerializeField] private Slider loadingGage;

    [Space]
    [Header("Title")]
    [SerializeField] private GameSettingController gameSetting;
    [SerializeField] private TMP_InputField mapNameInputfield;
    [SerializeField] private TMP_Dropdown tilePaletteDropdown;

    [Space]
    [SerializeField] private Library library;

    [Space]
    [Header("Setting")]
    [SerializeField] private GameObject settingObject;
    [SerializeField] private OptionUI optionUI;

    [Space]
    [Header("Game Scene")]
    [SerializeField] private GameObject clearView;
    [SerializeField] private GameObject gameOverView;
    [SerializeField] private RectTransform infoRect;
    [SerializeField] private TextMeshProUGUI roundInfo;
    [SerializeField] private DamageUI damageUI;
    [SerializeField] private DamagePool damageUIPool;
    [SerializeField] private Image flashImage;
    private EachRound nextRound;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private List<TextMeshProUGUI> statText;
    [SerializeField] private TextMeshProUGUI bonusText;

    [Header("Tower Panel")]
    [SerializeField] private List<BuildTowerButton> buildTowerButtons;

    [Header("Shop")]
    [SerializeField] private Shop shop;

    [Header("Tower Info")]
    [SerializeField] private TowerInfoPanel towerInfoPanel;

    [Header("ETC")]
    [SerializeField] private DescriptionPopup descPopup;

    [Space]
    [Header("Map Editor")]
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private MapEditorTile tilePrefab;
    [SerializeField] private RectTransform tileList;
    [SerializeField] private RectTransform[] sidePanel;
    private List<MapEditorTile> tiles;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static void GetTotal()
    {
        TotalProgress = 1;
    }
    public async Task Init()
    {
        optionUI.Init();

        await library.Init();
        library.Open(false);
        CurProgress++;

        await Title();
    }

    public void StartLoading()
    {
        loadingScene.SetActive(true);
    }

    public void Loading(float value)
    {
        loadingGage.value = value;
    }

    public void ChangeScene(int index)
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            bool b = (i == index);
            scenes[i].SetActive(b);
        }
        loadingScene.SetActive(false);
    }

    public async Task Title()
    {
        mapNameInputfield.text = "";
        await gameSetting.Init();

        while (TileManager.TilePaletteNames == null) await Task.Yield();
        tilePaletteDropdown.value = 0;
        tilePaletteDropdown.options.Clear();
        tilePaletteDropdown.AddOptions(TileManager.TilePaletteNames);

    }

    public void OpenSetting(bool b)
    {
        settingObject.SetActive(b);
        optionUI.gameObject.SetActive(false);
    }

    public void UpdateLanguage()
    {
        towerInfoPanel.UpdateLanguage();
        shop.UpdateLanguage();
        UpdateRoundInfo();
    }

    public void Clear()
    {
        clearView.SetActive(true);
    }
    public void GameOver()
    {
        gameOverView.SetActive(true);
    }

    public void SetDescription(Vector3 pos, string str)
    {
        descPopup.SetDescription(pos, str);
    }

    public void MoveDescription(Vector3 pos)
    {
        descPopup.MoveDescription(pos);
    }

    #region Library

    public void OpenLibrary()
    {
        library.Open(true);
    }

    public void UpdateLibrary()
    {
        library.UpdateLibrary();
    }

    #endregion

    #region Game Scene
    IEnumerator flashCoroutine;
    public void Flash(Color c, float time)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        c.a = 1 / 3;
        flashImage.color = c;
        flashCoroutine = Flashing(c, time);
        StartCoroutine(flashCoroutine);
    }

    private IEnumerator Flashing(Color c, float time)
    {
        flashImage.gameObject.SetActive(true);
        float origin = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            c.a = time / (origin * 3);
            flashImage.color = c;
            yield return null;
        }
        flashImage.gameObject.SetActive(false);
    }

    public void GameSetting(out CharacterType c, out List<DifficultyType> diff, out string map)
    {
        c = gameSetting.SelectedCharacter();
        diff = gameSetting.Difficulty();
        map = gameSetting.MapName();
    }

    public void StartGame()
    {
        clearView.SetActive(false);
        gameOverView.SetActive(false);

        for (int i = 0; i < buildTowerButtons.Count; i++)
        {
            buildTowerButtons[i].SetItem(null);
        }

        damageUIPool.Init(damageUI);
        shop.Init();

        OpenShop(true);
        UpdateTowerList();
        RerollAll();

        OpenShop(false);
        SelectTower(false);
    }

    public void EnemyDamaged(Vector3 pos, float damage)
    {
        damageUIPool.Pop(CameraController.Instance.Cam.WorldToScreenPoint(pos), damage);
    }

    public void PushDamageUI(GameObject go)
    {
        damageUIPool.Push(go);
    }

    #region Info Panel

    public void UpdateInfo(int life, int maxlife, int money, Character c)
    {
        lifeText.text = life.ToString() + " / " + maxlife.ToString();
        moneyText.text = money.ToString();
        expSlider.value = c.Exp;

        infoText.text =
            $"LEVEL: { string.Format("{0:##}", c.Level)}\n" +
            $"TYPE: {c.TypeString}";

        bonusText.text = string.Format("{0:#0}", c.BonusStat);

    }
    public void UpdateStat(Character c)
    {
        for (int i = 0; i < statText.Count; i++)
        {
            statText[i].text = string.Format("{0:#0}", c.Stat[(CharacterStatType)i]);
        }
        bonusText.text = string.Format("{0:#0}", c.BonusStat);

        //towerInfoPanel.UpdateBonusStat();
        towerInfoPanel.UpdateInfo();
    }

    public void NextRoundInfo(EachRound nextRoundInfo)
    {
        nextRound = nextRoundInfo;
        UpdateRoundInfo();
    }

    public void UpdateRoundInfo()
    {
        if (RoundController.Instance == null) return;

        string info = "";
        int height = 0;
        float amountDiff = RoundController.Instance.Difficulty;

        if (nextRound != null)
        {
            height = 30;
            foreach (var kvp in nextRound.unitData)
            {
                info += $"{EnemyManager.GetEnemy(kvp.Key).name} : {string.Format("{0:0}", kvp.Value * amountDiff)}\n";
                height += 50;
            }
        }

        infoRect.sizeDelta = new Vector2(infoRect.rect.width, height);
        roundInfo.text = info;
    }
    #endregion

    #region Towers Panel
    public void UpdateTowerList()
    {
        for (int i = 0; i < PlayerController.Instance.Towers.Count; i++)
        {
            Tower tower = PlayerController.Instance.Towers[i];

            buildTowerButtons[i].SetItem(tower);
        }
    }

    public void BuildTower(int id)
    {
        MapController.Instance.ReadyToBuild(id);
    }
    #endregion

    #region TowerInfoPanel

    public void OffTowerInfoPanel()
    {
        // TowerController내에서는 1단위로 이루어져있기 때문에 100의자리 소숫점은 없음.
        // 끄는 용도로 활용 가능.
        TowerController.Instance.SelectTower(Vector3.one / 100);
    }
    public void SelectTower(bool b, Tower tower = null)
    {
        towerInfoPanel.gameObject.SetActive(b);
        if (tower != null) towerInfoPanel.SetData(tower);
    }

    public bool PointInTowerInfo(Vector2 point)
    {
        if (towerInfoPanel.gameObject.activeSelf == false) return false;

        Rect rect = new Rect(towerInfoPanel.RectTransform.anchoredPosition, towerInfoPanel.RectTransform.rect.size);
        return rect.Contains(point);
    }

    public void ReinforceTower(int index, TowerStatType type)
    {
        towerInfoPanel.Reinforce(index, type);
    }

    #endregion

    #region Shop
    public void OpenShop(bool b)
    {
        shop.gameObject.SetActive(b);
        shop.UpdateProb();
    }

    public void RerollAll()
    {
        shop.RerollAll();
    }

    public void Reroll(TowerInfoItem item)
    {
        shop.Reroll(item);
        shop.UpdateAmount();
    }
    #endregion
    #endregion

    #region Map Edit Scene
    public string GetMapName()
    {
        return mapNameInputfield.text;
    }

    public string GetTileName()
    {
        return tilePaletteDropdown.options[tilePaletteDropdown.value].text;
    }
    public void EditMap(string mapName, string tileName)
    {
        mapNameText.text = mapName;
        LoadTiles(tileName);
        int count = Mathf.CeilToInt(tileList.childCount / 2);
        tileList.sizeDelta = new Vector2(300, 120 * count + 20 * (count - 1) + 30);
    }

    public void LoadTiles(string tileName)
    {
        if (tiles != null)
        {
            foreach (var tile in tiles)
            {
                Destroy(tile.gameObject);
            }
            tiles.Clear();
        }

        tiles = new List<MapEditorTile>();
        TilePalette tilePalette = TileManager.GetTilePalette(tileName);
        for (int i = 0; i < tilePalette.Tiles.Count; i++)
        {
            CustomRuleTile tile = tilePalette.Tiles[i];
            MapEditorTile metile = Instantiate(tilePrefab);

            metile.SetTile(tile);
            metile.transform.SetParent(tileList);
            tiles.Add(metile);
        }
    }
    public bool PointInTilePanel(Vector2 point)
    {
        for (int i = 0; i < sidePanel.Length; i++)
        {
            Vector2 pos = sidePanel[i].position;
            Rect rect = new Rect(pos + sidePanel[i].offsetMin, sidePanel[i].rect.size);
            if (rect.Contains(point)) return true;
        }
        return false;
    }

    #endregion
}
