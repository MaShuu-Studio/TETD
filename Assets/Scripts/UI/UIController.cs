using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        instance = this;
        DontDestroyOnLoad(gameObject);

        buildTowerButtons = new List<BuildTowerButton>();
    }

    [Header("Scenes")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private List<GameObject> scenes;

    [Header("Loading Scene")]
    [SerializeField] private Slider loadingGage;

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

        UpdateTowerList();
        RerollAll();

        OpenShop(false);
        SelectTower(false, null);
    }

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

    #region TowerInfoPanel
    public void SelectTower(bool b, Tower tower = null)
    {
        towerInfoPanel.gameObject.SetActive(b);
        if (tower != null) towerInfoPanel.SetData(tower);
    }

    public bool PointInTowerInfo(Vector2 point)
    {
        return towerInfoPanel.RectTransform.rect.Contains(point);
    }
    #endregion

    #region Shop
    public void OpenShop(bool b)
    {
        shop.gameObject.SetActive(b);
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
