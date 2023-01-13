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
    private TowerObject selectedTower = null;

    public bool ContainsTower(Vector3 pos)
    {
        return towers.ContainsKey(pos);
    }

    public bool BuildTower(int id, Vector3 pos)
    {
        if (ContainsTower(pos)) return false;

        Tower tower = TowerManager.GetTower(id);
        if (tower == null) return false;

        GameObject obj = PoolController.Pop(tower.name);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);

        TowerObject towerObj = obj.GetComponent<TowerObject>();
        towerObj.Init(tower, pos);

        towers.Add(pos, towerObj);

        return true;
    }

    public bool SelectTower(Vector3 pos)
    {
        if (selectedTower != null)
        {
            selectedTower.SelectTower(false);
            selectedTower = null;
        }

        if (ContainsTower(pos) == false) return false;

        TowerObject towerObj = towers[pos];
        towerObj.SelectTower(true);
        selectedTower = towerObj;

        return true;
    }

    public void RemoveTower(Vector3 pos)
    {
        if (ContainsTower(pos) == false) return;

        TowerObject tower = towers[pos];
        GameObject obj = towers[pos].gameObject;
        PoolController.Push(obj.name, obj);
        towers.Remove(pos);

        tower.RemoveTower();
    }

    public void RemoveEnemyObject(EnemyObject enemy)
    {
        foreach(var tower in towers.Values)
        {
            tower.RemoveEnemy(enemy);
        }
    }
}
