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

    private void Start()
    {
        buildTowerButtons = new List<BuildTowerButton>();

        Init();
    }

    public void Init()
    {
        for (int i = 0; i < 10; i++)
        {
            int id = -1;
            Tower tower = null;

            if (i < TowerManager.Keys.Count)
            {
                id = TowerManager.Keys[i];
                tower = TowerManager.GetTower(id);
            }

            GameObject go = Instantiate(buildTowerButtonPrefab.gameObject, buildTowerButtonsParent);
            BuildTowerButton button = go.GetComponent<BuildTowerButton>();

            button.SetItem(tower);
            buildTowerButtons.Add(button);
        }
    }

    public void BuildTower(int id)
    {
        GameController.Instance.ReadyToBuild(id);
    }
}
