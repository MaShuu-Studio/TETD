using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(CircleCollider2D))]
public class SplashPoint : Poolable
{
    private bool isActive;
    private Tower data;
    private float dmg;

    public override bool MakePrefab(int id)
    {
        isActive = false;
        transform.localScale = Vector3.zero;
        return true;
    }
    public void SetData(Tower data, float dmg)
    {
        this.data = data;
        this.dmg = dmg;
        isActive = true;

        transform.localScale = Vector3.one * data.Ability(AbilityType.SPLASH).value;

        StartCoroutine(Timer());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.tag == "Enemy")
            {
                EnemyObject obj = EnemyController.Instance.FindEnemy(collision.gameObject);
                EnemyController.Instance.EnemyAttacked(obj, data, dmg);
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
        TowerController.Instance.PushSplashPoint(gameObject);
    }

}
