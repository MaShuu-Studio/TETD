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

    private Queue<Tuple<EnemyObject, Tower, float>> enemyAttackedQueue;
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

        enemyAttackedQueue = new Queue<Tuple<EnemyObject, Tower, float>>();
        enemies = new List<EnemyObject>();
        road = new List<Vector3>();
        for (int i = 0; i < map.tilemap.enemyRoad.Count; i++)
        {
            Vector3 v = new Vector3(map.tilemap.enemyRoad[i].x, map.tilemap.enemyRoad[i].y);
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

    public void EnemyAttacked(EnemyObject enemy, Tower data, float dmg)
    {
        enemyAttackedQueue.Enqueue(new Tuple<EnemyObject, Tower, float>(enemy, data, dmg));

        if (flushCoroutine == null)
        {
            flushCoroutine = Flush();
            StartCoroutine(flushCoroutine);
        }
    }
    public void EnemyAttacked(List<EnemyObject> enemies, Tower data, float dmg)
    {
        if (enemies == null) return;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyObject enemy = enemies[i];
            EnemyAttacked(enemy, data, dmg);
        }
    }

    IEnumerator Flush()
    {
        while (enemyAttackedQueue.Count > 0)
        {
            int count = enemyAttackedQueue.Count;
            for (int i = 0; i < count; i++)
            {
                Tuple<EnemyObject, Tower, float> tuple;
                if (enemyAttackedQueue.TryDequeue(out tuple) == false) continue;

                EnemyObject enemy = tuple.Item1;
                Tower data = tuple.Item2;
                float dmg = tuple.Item3;

                if (enemies.Contains(enemy) == false) continue;
                enemy.Attacked(data, dmg);
            }
            yield return null;
        }

        flushCoroutine = null;
    }
}
