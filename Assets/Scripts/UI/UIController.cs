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
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    [Header("Tower Panel")]
    [SerializeField] private Transform buildTowerButtonsParent;
    [SerializeField] private BuildTowerButton buildTowerButtonPrefab;
    private List<BuildTowerButton> buildTowerButtons;

    [Header("Shop")]
    [SerializeField] private Shop shop;

    private void Start()
    {
        buildTowerButtons = new List<BuildTowerButton>();

        Init();
    }

    public void Init()
    {
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
        GameController.Instance.ReadyToBuild(id);
    }

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
}
