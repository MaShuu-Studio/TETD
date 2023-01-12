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

    public bool ContainsTower(Vector3 pos)
    {
        return towers.ContainsKey(pos);
    }

    public void BuildTower(int id, Vector3 pos)
    {
        if (ContainsTower(pos)) return;

        Tower tower = TowerManager.GetTower(id);
        if (tower == null) return;

        GameObject obj = PoolController.Pop(tower.name);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);

        TowerObject towerObj = obj.GetComponent<TowerObject>();
        towerObj.Init(tower, pos);

        towers.Add(pos, towerObj);
    }

    public void RemoveTower(Vector3 pos)
    {
        if (ContainsTower(pos) == false) return;

        GameObject obj = towers[pos].gameObject;
        PoolController.Push(obj.name, obj);
        towers.Remove(pos);
    }
}
