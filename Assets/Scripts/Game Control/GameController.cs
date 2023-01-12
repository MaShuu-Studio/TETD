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

    // 테스트용 임시 변수
    private int index = 0;
    private Camera cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
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

        if (TowerController.Instance.ReadyToBuild)
        {
            buildable = MapController.Instance.SelectTile(worldPos);
            if (click && buildable)
                TowerController.Instance.BuildTower(pos);
        }
        else MapController.Instance.OffSeletedTile();

        if (rclick)
            TowerController.Instance.RemoveTower(pos);

    }
}
