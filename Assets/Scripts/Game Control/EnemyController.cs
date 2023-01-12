using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance { get { return instance; } }
    private static EnemyController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private List<EnemyObject> enemies;
    private List<Vector3> road;
    private int enemyOrder;

    public void Init(Map map)
    {
        enemies = new List<EnemyObject>();
        road = new List<Vector3>();
        for (int i = 0; i < map.enemyRoad.Count; i++)
        {
            Vector3 v = new Vector3(map.enemyRoad[i].x, map.enemyRoad[i].y);
            v = MapController.Instance.GetMapPos(v);
            road.Add(v);
        }
    }

    public void AddEnemy(int id)
    {
        Enemy enemy = EnemyManager.GetEnemy(id);
        if (enemy == null) return;
        if (enemies == null) return;

        GameObject go = PoolController.Pop(enemy.name);
        EnemyObject enemyObj = go.GetComponent<EnemyObject>();

        enemies.Add(enemyObj);
        enemyObj.Init(enemy, road, -1 * enemyOrder++);
        enemyObj.transform.SetParent(transform);
    }

    public void RemoveEnemy(EnemyObject enemy)
    {
        if (enemies.Contains(enemy) == false) return;

        enemies.Remove(enemy);

        PoolController.Push(enemy.name, enemy.gameObject);
    }
}
