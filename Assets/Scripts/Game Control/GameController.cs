using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();

        MapController.Instance.LoadMap(MapUtil.LoadMap("RTD"));
        EnemyController.Instance.Init(MapController.Instance.GetMap());
        RoundController.Instance.Init(MapController.Instance.GetMap().name);
        PlayerController.Instance.Init();
    }

    // Update is called once per frame
    private void Update()
    {
        bool buildable = false;
        bool click = Input.GetMouseButtonDown(0);
        bool rclick = Input.GetMouseButtonDown(1);

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Vector3 pos = MapController.Instance.GetMapPos(worldPos);

        if (readyToBuild)
        {
            buildable = MapController.Instance.SelectTile(worldPos);
            if (click && buildable && TowerController.Instance.BuildTower(id, pos))
            {
                readyToBuild = false;
                id = 0;
            }
        }

        if (click && TowerController.Instance.SelectTower(pos))
        {
            MapController.Instance.OffSeletedTile();
            readyToBuild = false;
            id = 0;
        }

        if (rclick)
            TowerController.Instance.RemoveTower(pos);
    }

    // 임시 함수 해당 부분이 어디로 가야할지는 고민을 해야할 듯
    // 1. TowerController, 2. PlayerController, 3. GameController

    private bool readyToBuild;
    private int id;

    public void ReadyToBuild(int id)
    {
        this.id = id;
        readyToBuild = true;
    }
}
