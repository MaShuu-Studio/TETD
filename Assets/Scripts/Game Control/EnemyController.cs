using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

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

    private Queue<Tuple<EnemyObject, Element, float>> enemyDamagedQueue;
    private IEnumerator flushCoroutine;

    private List<EnemyObject> enemies;
    private List<Vector3> road;
    private int enemyOrder;

    public void Init(Map map)
    {
        enemyDamagedQueue = new Queue<Tuple<EnemyObject, Element, float>>();
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

        GameObject go = PoolController.Pop(id);
        EnemyObject enemyObj = go.GetComponent<EnemyObject>();

        enemies.Add(enemyObj);
        enemyObj.Init(road, -1 * enemyOrder++);
        enemyObj.transform.SetParent(transform);
    }

    public void RemoveEnemy(EnemyObject enemy)
    {
        if (enemies.Contains(enemy) == false) return;

        enemies.Remove(enemy);

        PoolController.Push(enemy.Id, enemy.gameObject);
    }

    public EnemyObject FindEnemy(GameObject go)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].gameObject == go)
            {
                return enemies[i];
            }
        }
        return null;
    }

    public void EnemyDamaged(EnemyObject enemy, Element element, float dmg)
    {
        if (enemy == null) return;

        enemyDamagedQueue.Enqueue(new Tuple<EnemyObject, Element, float>(enemy, element, dmg));
        if (flushCoroutine == null)
        {
            flushCoroutine = Flush();
            StartCoroutine(flushCoroutine);
        }
    }

    IEnumerator Flush()
    {
        int count = enemyDamagedQueue.Count;
        for (int i = 0; i < count; i++)
        {
            Tuple<EnemyObject, Element, float> tuple;
            if (enemyDamagedQueue.TryDequeue(out tuple) == false) continue;

            EnemyObject enemy = tuple.Item1;
            Element element = tuple.Item2;
            float dmg = tuple.Item3;

            if (enemies.Contains(enemy) == false) continue;
            enemy.Damaged(element, dmg);
        }
        yield return null;

        if (enemyDamagedQueue.Count > 0)
        {
            flushCoroutine = Flush();
            StartCoroutine(flushCoroutine);
        }
        else flushCoroutine = null;
    }
}
