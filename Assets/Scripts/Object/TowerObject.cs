using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    public Tower Data { get { return data; } }
    private Tower data;
    private IEnumerator delayCoroutine;
    private List<EnemyObject> enemies;

    private AttackPriority priority;
    public AttackPriority Priority { get { return priority; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";
    }

    public void Init(Tower data, Vector3 pos)
    {
        this.data = new Tower(data);

        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;

        rangeUI.transform.localPosition = range.transform.localPosition = Vector3.zero;
        rangeUI.transform.localScale = range.transform.localScale = Vector3.one * (1 + data.range * 2);

        rangeUI.SetActive(false);

        enemies = new List<EnemyObject>();
        priority = AttackPriority.FIRST;

        delayCoroutine = null;
    }

    public void SelectTower(bool b)
    {
        rangeUI.SetActive(b);
    }

    public void RemoveTower()
    {
        enemies.Clear();
        enemies = null;
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
    }

    public void AddEnemy(EnemyObject enemy)
    {
        enemies.Add(enemy);
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
            int index = SelectEnemy();

            EnemyController.Instance.EnemyDamaged(enemies[index], data.dmg);

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
    }

    private int SelectEnemy()
    {
        int selected = 0;

        EnemyObject selectedEnemy;
        for (int i = 1; i < enemies.Count; i++)
        {
            selectedEnemy = enemies[selected];
            EnemyObject enemy = enemies[i];

            if (priority == AttackPriority.FIRST || priority == AttackPriority.LAST)
            {
                int selectedOrder = selectedEnemy.Order;
                int order = enemy.Order;

                selected = SelectIndex(selected, i, selectedOrder, order, priority == AttackPriority.FIRST);
            }
            else
            {
                float selectedHp = selectedEnemy.Hp;
                float hp = enemy.Hp;

                selected = SelectIndex(selected, i, selectedHp, hp, priority == AttackPriority.STRONG);
            }
        }
        selectedEnemy = enemies[selected];
        return selected;
    }

    private int SelectIndex(int firstIndex, int secondIndex, int firstValue, int secondValue, bool large)
    {
        if (firstValue == secondValue) return firstIndex;

        if (firstValue > secondValue && large) return firstIndex;
        if (firstValue < secondValue && !large) return firstIndex;

        return secondIndex;
    }

    private int SelectIndex(int firstIndex, int secondIndex, float firstValue, float secondValue, bool large)
    {
        if (firstValue == secondValue) return firstIndex;

        if (firstValue > secondValue && large) return firstIndex;
        if (firstValue < secondValue && !large) return firstIndex;

        return secondIndex;
    }
}
