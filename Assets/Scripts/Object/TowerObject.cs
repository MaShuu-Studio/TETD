using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    private Tower data;
    private IEnumerator delayCoroutine;
    private List<EnemyObject> enemies;

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
            // 우선순위에 따라 선택
            int index = Random.Range(0, enemies.Count);

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
}
