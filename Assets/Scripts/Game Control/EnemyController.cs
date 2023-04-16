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

    private Queue<Tuple<EnemyObject, Tower>> enemyAttackedQueue;
    private IEnumerator flushCoroutine;

    public int EnemyAmount { get { return enemies.Count; } }
    private List<EnemyObject> enemies;
    private List<Vector3> road;
    private int enemyOrder;

    private float hpDif;
    private float speedDif;

    public void Init(Map map, List<DifficultyType> difficulties)
    {
        hpDif = 1;
        speedDif = 1;

        if (difficulties.Contains(DifficultyType.HP)) hpDif = 1.5f;
        if (difficulties.Contains(DifficultyType.SPEED)) speedDif = 1.5f;

        enemyAttackedQueue = new Queue<Tuple<EnemyObject, Tower>>();
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
        enemyObj.Init(road, -1 * enemyOrder++, hpDif, speedDif);
        enemyObj.transform.SetParent(transform);
    }

    public void RemoveEnemy(EnemyObject enemy)
    {
        TowerController.Instance.RemoveEnemyObject(enemy);

        if (enemies.Contains(enemy) == false) return;

        enemies.Remove(enemy);

        PoolController.Push(enemy.Id, enemy.gameObject);
    }

    public void EnemyArrive(EnemyObject enemy)
    {
        PlayerController.Instance.Damaged(1);
        RemoveEnemy(enemy);
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

    public void EnemyAttacked(EnemyObject enemy, Tower data)
    {
        enemyAttackedQueue.Enqueue(new Tuple<EnemyObject, Tower>(enemy, data));

        if (flushCoroutine == null)
        {
            flushCoroutine = Flush();
            StartCoroutine(flushCoroutine);
        }
    }
    public void EnemyAttacked(List<EnemyObject> enemies, Tower data)
    {
        if (enemies == null) return;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyObject enemy = enemies[i];
            EnemyAttacked(enemy, data);
        }
    }

    IEnumerator Flush()
    {
        while (enemyAttackedQueue.Count > 0)
        {
            int count = enemyAttackedQueue.Count;
            for (int i = 0; i < count; i++)
            {
                Tuple<EnemyObject, Tower> tuple;
                if (enemyAttackedQueue.TryDequeue(out tuple) == false) continue;

                EnemyObject enemy = tuple.Item1;
                Tower data = tuple.Item2;

                if (enemies.Contains(enemy) == false) continue;
                enemy.Attacked(data);
            }
            yield return null;
        }

        flushCoroutine = null;
    }
}
