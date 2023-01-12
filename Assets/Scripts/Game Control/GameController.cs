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

        if (readyToBuild)
        {
            buildable = MapController.Instance.SelectTile(worldPos);
            if (click && buildable)
                TowerController.Instance.BuildTower(id, pos);
        }
        else MapController.Instance.OffSeletedTile();

        if (rclick)
            TowerController.Instance.RemoveTower(pos);

    }

    private string selectedTower;
    private bool readyToBuild;
    private int id;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        bool firstTower = GUI.Button(new Rect(0, 0, 100, 50), TowerManager.Keys[0].ToString(), style);
        bool secondTower = GUI.Button(new Rect(0, 60, 100, 50), TowerManager.Keys[1].ToString(), style);
        bool notSelect = GUI.Button(new Rect(0, 120, 100, 50), "NOT SELECT", style);
        if (firstTower)
        {
            readyToBuild = true;
            id = TowerManager.Keys[0];
        }
        if (secondTower)
        {
            readyToBuild = true;
            id = TowerManager.Keys[1];
        }
        if (notSelect) readyToBuild = false;

        if (readyToBuild)
        {
            selectedTower = id.ToString();
        }
        else
        {
            selectedTower = "NONE";
        }

        GUI.Label(new Rect(120, 0, 100, 50), selectedTower, style);

    }
}
