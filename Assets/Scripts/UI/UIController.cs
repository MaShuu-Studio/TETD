using EnumData;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        Loading();

        instance = this;
        DontDestroyOnLoad(gameObject);
        OpenSetting(false);
    }

    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject[] scenes;
    public int OpenScene { get { return openScene; } }
    private int openScene;

    [Space]
    [Header("Loading Scene")]
    [SerializeField] private Slider loadingGage;
    [SerializeField] private TextMeshProUGUI loadingProgressText;

    [Space]
    [Header("Title")]
    [SerializeField] private GameSettingController gameSetting;
    [SerializeField] private GameObject[] titleWindows;

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
    [SerializeField] private TextMeshProUGUI[] statText;
    [SerializeField] private TextMeshProUGUI bonusText;

    [Space]
    [SerializeField] private TowerPanel towerPanel;

    [Space]
    [SerializeField] private Shop shop;

    [Space]
    [SerializeField] private TowerInfoPanel towerInfoPanel;


    [Space]
    [Header("Custom Editor")]
    [SerializeField] private CustomEditor customEditor;
    [SerializeField] private GameObject[] editorUI; // main, map, unit, round
    [SerializeField] private Image[] towerImages;
    [SerializeField] private GameObject towerEtcSymbol;
    [SerializeField] private Image[] enemyImages;
    [SerializeField] private GameObject enemyEtcSymbol;
    [SerializeField] private TextMeshProUGUI selectedMapNameText;
    [SerializeField] private MapInfoIcon selectedMapInfo;
    [SerializeField] private TMP_InputField newMapInput;
    private int selectedMap;

    [Space]
    [Header("Map Editor")]
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private MapEditorPanel mapEditorPanel;

    [Space]
    [Header("Unit Editor")]
    [SerializeField] private UnitEditor unitEditor;
    private static int uiLayer = 5;
    private static int pointOverUILayer = 6;

    [Header("ETC")]
    [SerializeField] private DescriptionPopup descPopup;
    [SerializeField] private ErrorLog errorLog;
    [SerializeField] private TutorialController tutorialController;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static void GetTotal()
    {
        TotalProgress = 1;
    }

    public async Task Init()
    {
        for (int i = 0; i < scenes.Length; i++)
            scenes[i].SetActive(true);

        for (int i = 0; i < titleWindows.Length; i++)
            titleWindows[i].SetActive(true);

        await library.Init();
        library.Open(false);

        shop.Init();
        towerInfoPanel.Init();

        for (int i = 0; i < editorUI.Length; i++)
        {
            editorUI[i].SetActive(true);
        }
        mapEditorPanel.Init();
        unitEditor.Init();
        customEditor.Init();

        await gameSetting.Init();
        tutorialController.Init();

        await optionUI.Init();
        CurProgress++;
        Title();
    }

    private void Update()
    {
        // scenes| 0: Title, 1: Game Scene, 2: Map Editor, 3: Unit Editor, 4: Round Editor
        if (openScene == 1)
        {
            // Game Scene이 열려 있을 때.
            bool open = Input.GetButtonDown("Shop");
            if (open)
                OpenShop(!shop.gameObject.activeSelf);

            // 상점이 열려있다면 리롤버튼 작동
            if (shop.gameObject.activeSelf)
            {
                bool reroll = Input.GetButtonDown("Shop Reroll");
                bool prob = Input.GetButtonDown("Shop Prob");

                if (reroll) RerollAll();
                if (prob) shop.OnOffRemainInfo();
            }
        }
    }

    public void Loading(bool b = true)
    {
        loadingScene.SetActive(b);
    }

    public void Loading(float value, string progress)
    {
        loadingGage.value = value;
        loadingProgressText.text = progress;
    }

    public void ChangeScene(int index)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            bool b = (i == index);
            scenes[i].SetActive(b);
        }
        loadingScene.SetActive(false);
        openScene = index;
    }

    public void Title()
    {
        for (int i = 0; i < titleWindows.Length; i++)
            titleWindows[i].SetActive(false);
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
        library.UpdatePage();
        unitEditor.UpdateLanguage();
        gameSetting.UpdateLanage();
        tutorialController.UpdateLauguage();
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

    public void ErrorLog(string log, string str)
    {
        errorLog.Log(log, str);
    }

    public void SetDescription(Vector3 pos, string str)
    {
        descPopup.SetDescription(pos, str);
    }

    public void MoveDescription(Vector3 pos)
    {
        descPopup.MoveDescription(pos);
    }

    public static bool PointOverUI(GameObject go)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject == go) return true;
        }
        return false;
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

        damageUIPool.Init(damageUI);
        towerPanel.StartGame();
        shop.StartGame();

        OpenShop(true);
        UpdateTowerList();
        RerollAll();

        OpenShop(false);
        SelectTower(false);
        
        ShowTutorial(0);
    }

    public void EnemyDamaged(Vector3 pos, float damage)
    {
        damageUIPool.Pop(CameraController.Instance.Cam.WorldToScreenPoint(pos), damage);
    }

    public void PushDamageUI(GameObject go)
    {
        damageUIPool.Push(go);
    }

    public void ShowTutorial(int index)
    {
        tutorialController.ShowTutorial(index);
    }

    public void SetTutorialProgress(bool b)
    {
        tutorialController.SetProgress(b);
    }

    public void EndTutorial()
    {
        optionUI.EndTutorial();
    }

    #region Info Panel

    public void UpdateInfo(int life, int maxlife, int money, Character c)
    {
        lifeText.text = life.ToString() + " / " + maxlife.ToString();
        moneyText.text = money.ToString();
        expSlider.value = c.Exp;

        infoText.text =
            $"LEVEL: { string.Format("{0:##}", c.Level)}\n" +
            $"TYPE: {c.TypeString} ({string.Format("{0:##}", PlayerController.Instance.GetAbility())})";

        bonusText.text = string.Format("{0:#0}", c.BonusStat);

    }
    public void UpdateStat(Character c)
    {
        for (int i = 0; i < statText.Length; i++)
        {
            statText[i].text = string.Format("{0:#0}", c.GetStat((Element)(i / 2), (Character.ElementStatType)(i % 2)));
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
        towerPanel.UpdateTowersInfo();
    }

    public void SelectTower(int index)
    {
        towerPanel.SelectTower(index);
    }

    public void ReadyToBuildTower(int id)
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
        else towerInfoPanel.gameObject.SetActive(false);
    }

    public bool PointInTowerInfo(Vector2 point)
    {
        if (towerInfoPanel.gameObject.activeSelf == false) return false;

        return PointOverUI(towerInfoPanel.gameObject);
    }

    public void ReinforceTower(int index, int id)
    {
        towerInfoPanel.Reinforce(index, id);
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

    #region Custom Editor

    public void ChangeCustomEditorUI(int index)
    {
        for (int i = 0; i < editorUI.Length; i++)
            editorUI[i].SetActive(i == index);
    }

    public void EditCustomData()
    {
        ChangeCustomEditorUI(0);

        int i = 0;
        towerEtcSymbol.SetActive(false);
        foreach (var sprite in CustomDataManager.EditingTowerSprites.Values)
        {
            if (sprite == null) continue;

            if (i >= towerImages.Length)
            {
                towerEtcSymbol.SetActive(true);
                break;
            }
            towerImages[i].sprite = sprite;
            ((RectTransform)towerImages[i].transform).sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) * 3;
            
            i++;
        }
        for (; i < towerImages.Length; i++)
            towerImages[i].gameObject.SetActive(false);

        i = 0;
        enemyEtcSymbol.SetActive(false);
        foreach (var sprite in CustomDataManager.EditingEnemySprites.Values)
        {
            if (sprite == null) continue;

            if (i >= enemyImages.Length)
            {
                enemyEtcSymbol.SetActive(true);
                break;
            }
            enemyImages[i].sprite = sprite;
            ((RectTransform)enemyImages[i].transform).sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) * 3;
        
            i++;
        }
        for (; i < enemyImages.Length; i++)
            enemyImages[i].gameObject.SetActive(false);



        selectedMap = -1;
        SelectMap(true);
    }

    public void SelectMap(bool next)
    {
        if (next) selectedMap++;
        else selectedMap--;

        if (selectedMap > CustomDataManager.EditingMapNames.Count) selectedMap = 0;
        else if (selectedMap < 0) selectedMap = CustomDataManager.EditingMapNames.Count - 1;

        UpdateMap();
    }

    public void UpdateMap()
    {
        // 가장 끝 번호는 신규 맵 생성
        if (selectedMap >= CustomDataManager.EditingMapNames.Count)
        {
            newMapInput.gameObject.SetActive(true);
            selectedMapInfo.gameObject.SetActive(false);
            selectedMapNameText.text = "CREATE NEW MAP";
            newMapInput.text = "";
        }
        else
        {
            newMapInput.gameObject.SetActive(false);
            selectedMapInfo.gameObject.SetActive(true);
            string mapName = CustomDataManager.EditingMapNames[selectedMap];
            selectedMapNameText.text = mapName;
            selectedMapInfo.SetIcon(CustomDataManager.EditingMapData[mapName]);
        }
    }

    public void UpdateCustomData()
    {
        // 라이브러리와 게임준비화면을 재로드
        library.UpdateLibrary();
        gameSetting.UpdateMaps();
    }

    #region Map Edit Scene

    public Map GetMap()
    {
        if (selectedMap < CustomDataManager.EditingMapNames.Count)
            return CustomDataManager.EditingMapData[CustomDataManager.EditingMapNames[selectedMap]];
        return null;
    }
    public string GetMapName()
    {
        string mapName = (selectedMap < CustomDataManager.EditingMapNames.Count) ? selectedMapNameText.text : newMapInput.text;
        return mapName;
    }

    public void EditMap(string mapName)
    {
        ChangeCustomEditorUI(1);
        mapNameText.text = mapName;
    }

    public bool PointInMapEditPanel()
    {
        return PointOverUI(mapEditorPanel.gameObject);
    }

    #endregion

    #region Unit Editor
    public void EditUnit()
    {
        ChangeCustomEditorUI(2);
        unitEditor.EditUnit();
    }

    public void UpdatePoster(Tower data)
    {
        unitEditor.UpdatePoster(data);
    }

    public void UpdatePoster(Enemy data)
    {
        unitEditor.UpdatePoster(data);
    }
    #endregion

    #region Round Editor
    #endregion
    #endregion
}
