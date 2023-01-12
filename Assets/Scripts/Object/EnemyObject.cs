using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private List<Vector3> road;
    private float speed;
    private int destRoad;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Enemy";
    }

    public void Init(List<Vector3> road, int order)
    {
        this.road = road;

        speed = 5;
        transform.position = road[0];
        destRoad = 1;

        spriteRenderer.sortingOrder = order;

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
