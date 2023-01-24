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
    public TowerObject SelectedTower { get { return selectedTower; } }

    public bool ContainsTower(Vector3 pos)
    {
        return towers.ContainsKey(pos);
    }

    public bool BuildTower(int id, Vector3 pos)
    {
        if (ContainsTower(pos)) return false;

        Tower tower = TowerManager.GetTower(id);
        if (tower == null) return false;
        if (PlayerController.Instance.Buy(tower.cost) == false) return false;

        GameObject obj = PoolController.Pop(tower.id);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);

        TowerObject towerObj = obj.GetComponent<TowerObject>();
        towerObj.Build(pos);

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

        Tower tower = null;
        bool select = false;

        if (ContainsTower(pos))
        {
            TowerObject towerObj = towers[pos];
            towerObj.SelectTower(true);
            selectedTower = towerObj;

            tower = towerObj.Data;
            select = true;
        }
        UIController.Instance.SelectTower(select, tower);
        return select;
    }

    public void SellTower()
    {
        if (selectedTower == null) return;

        int value = selectedTower.Data.Value();
        GameObject obj = selectedTower.gameObject;
        PoolController.Push(selectedTower.Id, obj);
        towers.Remove(selectedTower.Pos);

        PlayerController.Instance.Reward(0, value);
        selectedTower.RemoveTower();
        selectedTower = null;

        UIController.Instance.SelectTower(false);
    }

    public void RemoveEnemyObject(EnemyObject enemy)
    {
        foreach(var tower in towers.Values)
        {
            tower.RemoveEnemy(enemy);
        }
    }
}
