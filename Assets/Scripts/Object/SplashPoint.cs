using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(CircleCollider2D))]
public class SplashPoint : Poolable
{
    private bool isActive;
    private Tower data;
    public override bool MakePrefab(int id)
    {
        isActive = false;
        transform.localScale = Vector3.zero;
        return true;
    }

    public void SetData(Tower data)
    {
        this.data = data;
        isActive = true;

        transform.localScale = Vector3.one * data.Stat(TowerStatType.SPLASH);

        StartCoroutine(Timer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.tag == "Enemy")
            {
                EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
                EnemyController.Instance.EnemyAttacked(obj, data);
            }
        }
    }

    private IEnumerator Timer()
    {
        float time = 0;
        while(time < 0.1f)
        {
            if (GameController.Instance.Paused)
            {
                yield return null;
                continue;
            }
            time += Time.deltaTime;
            yield return null;
        }
        isActive = false;
        transform.localScale = Vector3.zero;
        TowerController.Instance.PushSplash(gameObject);
    }
}
