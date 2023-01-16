using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyObject : Poolable
{
    private SpriteRenderer spriteRenderer;

    private Enemy data;

    private List<Vector3> road;
    private float hp;
    private float speed;
    private int destRoad;
    public int Order { get; private set; }
    public float Hp { get { return hp; } }

    public override bool MakePrefab(int id)
    {
        this.id = id;
        Enemy data = EnemyManager.GetEnemy(id);
        if (data == null) return false;

        amount = 2;

        this.data = new Enemy(data);
        gameObject.name = data.name;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Enemy";

        tag = "Enemy";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        return true;
    }

    public void Init(List<Vector3> road, int order)
    {
        this.road = road;

        hp = data.hp;
        speed = data.speed;

        transform.position = road[0];
        destRoad = 1;

        spriteRenderer.sortingOrder = Order = order;

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (destRoad < road.Count)
        {
            int curRoad = destRoad - 1;
            Vector3 v = Vector3.Normalize(road[destRoad] - road[curRoad]);
            bool destlargeX = road[destRoad].x > road[curRoad].x;
            bool destlargeY = road[destRoad].y > road[curRoad].y;

            while (CompareVector(transform.position, road[destRoad], destlargeX, destlargeY))
            {
                yield return null;
                Vector3 moveAmount = v * speed * Time.deltaTime;
                transform.position += moveAmount;
            }
            transform.position = road[destRoad];

            destRoad++;
        }
        ArriveDest();
    }

    public void Damaged(float dmg)
    {
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] {name} Damaged {dmg} | HP: {hp}");
#endif
        hp -= dmg;

        if (hp <= 0)
        {
            TowerController.Instance.RemoveEnemyObject(this);
            EnemyController.Instance.RemoveEnemy(this);
        }
    }

    private void ArriveDest()
    {
        EnemyController.Instance.RemoveEnemy(this);
    }

    // first, second, secondLargeX, secondLargeY
    private bool CompareVector(Vector3 first, Vector3 second, bool slx, bool sly)
    {
        return (((slx && transform.position.x <= road[destRoad].x) || (!slx && transform.position.x >= road[destRoad].x))
            && ((sly && transform.position.y <= road[destRoad].y) || (!sly && transform.position.y >= road[destRoad].y)));
    }
}
