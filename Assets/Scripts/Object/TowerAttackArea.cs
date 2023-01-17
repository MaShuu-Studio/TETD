using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TowerAttackArea : MonoBehaviour
{
    [SerializeField] private TowerObject tower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
            if (obj != null) tower.AddEnemy(obj);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
            tower.RemoveEnemy(obj);
        }
    }
}
