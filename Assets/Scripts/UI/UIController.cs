using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;
using System;

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

        buildTowerButtons = new List<BuildTowerButton>();
    }

    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private List<GameObject> scenes;

    [Header("Loading Scene")]
    [SerializeField] private Slider loadingGage;

    [Header("Title")]
    [SerializeField] private GameSettingController gameSetting;

    [Space]
    [Header("Game Scene")]
    [SerializeField] private RectTransform infoRect;
    [SerializeField] private TextMeshProUGUI roundInfo;
    [SerializeField] private DamageUI damageUI;
    [SerializeField] private DamagePool damageUIPool;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private List<TextMeshProUGUI> statText;
    [SerializeField] private TextMeshProUGUI bonusText;

    [Header("Tower Panel")]
    [SerializeField] private Transform buildTowerButtonsParent;
    [SerializeField] private BuildTowerButton buildTowerButtonPrefab;
    private List<BuildTowerButton> buildTowerButtons;

    [Header("Shop")]
    [SerializeField] private Shop shop;

    [Header("Tower Info")]
    [SerializeField] private TowerInfoPanel towerInfoPanel;


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

    public void SettingGame()
    {
        GameController.Instance.SettingGame(gameSetting.SelectedCharacter(), gameSetting.Difficulty(), gameSetting.MapName());
    }

    #region Game Scene
    public void StartGame()
    {
        for (int i = 0; i < buildTowerButtons.Count; i++)
        {
            Destroy(buildTowerButtons[i].gameObject);
        }
        buildTowerButtons.Clear();

        for (int i = 0; i < 10; i++)
        {
            Tower tower = null;

            GameObject go = Instantiate(buildTowerButtonPrefab.gameObject, buildTowerButtonsParent);
            BuildTowerButton button = go.GetComponent<BuildTowerButton>();

            button.SetItem(tower);
            buildTowerButtons.Add(button);
        }

        damageUIPool.Init(damageUI);

        UpdateTowerList();
        RerollAll();

        OpenShop(false);
        SelectTower(false, null);
    }

    public void EnemyDamaged(Vector3 pos, float damage)
    {
        damageUIPool.Pop(MapController.Instance.PosToScreen(pos), damage);
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

        towerInfoPanel.UpdateBonusStat();
    }

    public void NextRoundInfo(EachRound nextRoundInfo)
    {
        string info = "";
        int height = 0;
        if (nextRoundInfo != null)
        {
            height = 30;
            foreach (var kvp in nextRoundInfo.unitData)
            {
                info += $"{EnemyManager.GetEnemy(kvp.Key).name.Substring(6)} : {kvp.Value}\n";
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
    public void SelectTower(bool b, Tower tower = null)
    {
        towerInfoPanel.gameObject.SetActive(b);
        if (tower != null) towerInfoPanel.SetData(tower);
    }

    public bool PointInTowerInfo(Vector2 point)
    {
        Rect rect = new Rect(towerInfoPanel.RectTransform.anchorMin, towerInfoPanel.RectTransform.rect.size);
        return rect.Contains(point);
    }

    public void ReinforceTower(TowerMainStatType type)
    {
        towerInfoPanel.Reinforce(type);
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
    }
    #endregion
    #endregion
}
