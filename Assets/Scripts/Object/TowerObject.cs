using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : Poolable
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    public Vector3 Pos { get; private set; }
    public Tower Data { get { return data; } }
    private Tower data;
    private IEnumerator attackCoroutine;
    private IEnumerator miningCoroutine;
    private PriorityQueue<EnemyObject> enemies;

    private AttackPriority priority;
    public AttackPriority Priority { get { return priority; } }

    public override bool MakePrefab(int id)
    {
        this.id = id;
        Tower data = TowerManager.GetTower(id);
        if (data == null) return false;

        amount = 2;

        gameObject.name = data.name;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        rangeUI.transform.localPosition = range.transform.localPosition = Vector3.zero;

        return true;
    }

    public void Build(Vector3 pos)
    {
        Pos = pos;

        Tower data = TowerManager.GetTower(id);
        this.data = new Tower(data);

        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;

        UpdateDistnace();

        rangeUI.SetActive(false);

        enemies = new PriorityQueue<EnemyObject>();

        priority = AttackPriority.FIRST;

        attackCoroutine = null;
        miningCoroutine = null;
        if (data.Stat(TowerStatType.GOLDMINE) != 0)
        {
            miningCoroutine = Mining();
            StartCoroutine(miningCoroutine);
        }
    }

    public void SelectTower(bool b)
    {
        rangeUI.SetActive(b);
    }

    public void RemoveTower()
    {
        enemies = null;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
        }
    }

    public void UpdateDistnace()
    {
        rangeUI.transform.localScale = range.transform.localScale = Vector3.one * (1 + data.Stat(TowerStatType.DISTANCE) * 2);
    }

    public void AddEnemy(EnemyObject enemy)
    {
        enemies.Enqueue(enemy, GetPriority(enemy));
        if (attackCoroutine == null)
        {
            attackCoroutine = Attack();
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
            int targetAmount = (int)data.Stat(TowerStatType.MULTISHOT);
            if (targetAmount == 0) targetAmount = 1;
            SoundController.PlayAudio(id);
            EnemyController.Instance.EnemyAttacked(enemies.Get(targetAmount), data);

            float delayTime = 0;
            float delay = 1 / Stat(TowerStatType.ATTACKSPEED);
            while (delayTime < delay)
            {
                delayTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        attackCoroutine = null;
    }

    private IEnumerator Mining()
    {
        while (true)
        {

            float delayTime = 0;
            float delay = 1 / Stat(TowerStatType.ATTACKSPEED);
            while (delayTime < delay)
            {
                delayTime += Time.deltaTime;
                yield return null;
            }
            int value = (int)data.Stat(TowerStatType.GOLDMINE);
            PlayerController.Instance.Reward(0, value);
            yield return null;
        }
    }

    private float Stat(TowerStatType type)
    {
        float value = data.Stat(type);

        value += BonusStat(type);

        return value;
    }

    public float BonusStat(TowerStatType type)
    {
        float value;
        int stat = 0;

        if (type == TowerStatType.DAMAGE && PlayerController.Instance.Type == CharacterType.POWER)
            stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);

        else if (type == TowerStatType.ATTACKSPEED && PlayerController.Instance.Type == CharacterType.ATTACKSPEED)
            stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);

        stat += PlayerController.Instance.BonusElement(data.element);

        float percent = stat / 100f;
        value = data.Stat(type) * percent;

        return value;
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
        else if (priority == AttackPriority.ELEMENT)
        {
            if (enemy.Data.WeakElement() == data.element) prior = 100;
            else if (enemy.Data.StrongElement() != data.element) prior = 10;

            prior += enemy.Order / 10000f;
        }

        return prior;
    }
}
