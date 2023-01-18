using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : Poolable
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    public Tower Data { get { return data; } }
    private Tower data;
    private IEnumerator delayCoroutine;
    private PriorityQueue<EnemyObject> enemies;

    private AttackPriority priority;
    public AttackPriority Priority { get { return priority; } }

    public override bool MakePrefab(int id)
    {
        this.id = id;
        Tower data = TowerManager.GetTower(id);
        if (data == null) return false;

        amount = 2;

        this.data = new Tower(data);
        gameObject.name = data.name;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        rangeUI.transform.localPosition = range.transform.localPosition = Vector3.zero;
        rangeUI.transform.localScale = range.transform.localScale = Vector3.one * (1 + data.range * 2);

        return true;
    }

    public void Build(Vector3 pos)
    {
        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;

        rangeUI.SetActive(false);

        enemies = new PriorityQueue<EnemyObject>();

        priority = AttackPriority.FIRST;

        delayCoroutine = null;
    }

    public void SelectTower(bool b)
    {
        rangeUI.SetActive(b);
    }

    public void RemoveTower()
    {
        enemies = null;
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
    }

    public void AddEnemy(EnemyObject enemy)
    {
        enemies.Enqueue(enemy, GetPriority(enemy));
        if (delayCoroutine == null)
        {
            delayCoroutine = Attack();
            StartCoroutine(Attack());
        }
    }
    public void RemoveEnemy(EnemyObject enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    private IEnumerator Attack()
    {
        while (enemies.Count > 0)
        {
            SoundController.PlayAudio(id);
            EnemyController.Instance.EnemyDamaged(enemies.Get(), data.dmg);

            float delayTime = 0;
            float delay = 1 / data.attackspeed;
            while (delayTime < delay)
            {
                delayTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        delayCoroutine = null;
    }

    public void ChangePriority(AttackPriority type)
    {
        priority = type;

        PriorityQueue<EnemyObject> tmp = new PriorityQueue<EnemyObject>();
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyObject enemy = enemies.Dequeue();
            tmp.Enqueue(enemy, GetPriority(enemy));
        }

        enemies = null;
        enemies = tmp;
    }

    public float GetPriority(EnemyObject enemy)
    {
        float prior = 0;
        if (priority == AttackPriority.FIRST)
            prior = enemy.Order;
        else if (priority == AttackPriority.LAST)
            prior = enemy.Order * -1;
        else if (priority == AttackPriority.STRONG)
            prior = enemy.Hp;
        else if (priority == AttackPriority.WEAK)
            prior = enemy.Hp * -1;

        return prior;
    }
}
