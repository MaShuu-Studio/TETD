using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TowerAttackArea : MonoBehaviour
{
    private TowerObject tower;
    private bool isbuff;

    public void Init(TowerObject tower, bool isbuff)
    {
        this.isbuff = isbuff;
        this.tower = tower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
            if (obj != null) tower.AddEnemy(obj);
        }

        if (isbuff && collision.tag == "Tower")
        {
            TowerObject obj = TowerController.Instance.FindTower(collision.gameObject);
            if (obj != null) tower.AddTower(obj);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
            if (obj != null) tower.RemoveEnemy(obj);
        }

        if (isbuff && collision.tag == "Tower")
        {
            TowerObject obj = TowerController.Instance.FindTower(collision.gameObject);
            if (obj != null) tower.RemoveTower(obj);
        }
    }
}
