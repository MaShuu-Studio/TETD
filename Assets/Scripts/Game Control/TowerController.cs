using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public static TowerController Instance { get { return instance; } }
    private static TowerController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private Dictionary<Vector3, TowerObject> towers = new Dictionary<Vector3, TowerObject>();
    private bool readyToBuild;
    public bool ReadyToBuild { get { return readyToBuild; } }

    public bool ContainsTower(Vector3 pos)
    {
        return towers.ContainsKey(pos);
    }
    public void BuildTower(Vector3 pos)
    {
        if (ContainsTower(pos)) return;

        selectedTower = towerDatas[index];
        GameObject obj = PoolController.Pop(selectedTower);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);

        TowerObject tower = obj.GetComponent<TowerObject>();
        tower.Arrange(pos);

        towers.Add(pos, tower);
    }

    public void RemoveTower(Vector3 pos)
    {
        if (ContainsTower(pos) == false) return;

        GameObject obj = towers[pos].gameObject;
        Destroy(towers[pos]);
        PoolController.Push(obj.name, obj);
        towers.Remove(pos);
    }

    // 테스트용
    private string[] towerDatas = { "TOWER-TEST1", "TOWER-TEST2" }; // 실제 타워 목록
    private string selectedTower;
    private int index = 0;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        bool firstTower = GUI.Button(new Rect(0, 0, 100, 50), "TEST1", style);
        bool secondTower = GUI.Button(new Rect(0, 60, 100, 50), "TEST2", style);
        bool notSelect = GUI.Button(new Rect(0, 120, 100, 50), "NOT SELECT", style);
        if (firstTower)
        {
            readyToBuild = true;
            index = 0;
        }
        if (secondTower)
        {
            readyToBuild = true;
            index = 1;
        }
        if (notSelect) readyToBuild = false;

        if (readyToBuild)
        {
            selectedTower = towerDatas[index];
        }
        else
        {
            selectedTower = "NONE";
        }

        GUI.Label(new Rect(120, 0, 100, 50), selectedTower, style);

    }
}
