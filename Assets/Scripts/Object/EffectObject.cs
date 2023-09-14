using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : Poolable
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    private float spf;

    // ��Ʈ�� ��� ���ڸ��� 1, ������ ��� 2.
    public const int CheckBuffEffect = 100000000;

    public override bool MakePrefab(int id)
    {
        this.id = id;
        amount = 5;

        spriteRenderer = GetComponent<SpriteRenderer>();

        int check = id / CheckBuffEffect;
        id = id % CheckBuffEffect;

        if (check == 1)
        {
            if (TowerManager.HitEffects.ContainsKey(id))
            {
                sprites = TowerManager.HitEffects[id];
                spf = TowerManager.GetTower(id).effectspf;
                spriteRenderer.sprite = sprites[0];
                return true;
            }
        }
        else
        {
            if (TowerManager.BuffEffects.ContainsKey(id))
            {
                sprites = TowerManager.BuffEffects[id];
                spf = TowerManager.GetTower(id).effectspf;
                spriteRenderer.sprite = sprites[0];
                return true;
            }
        }

        return false;
    }
    IEnumerator Animation()
    {
        int number = 0;
        float time = 0;
        float frameTime = spf;
        while (true)
        {
            // ��������Ʈ ����
            spriteRenderer.sprite = sprites[number++];

            while (time < frameTime)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                time += Time.deltaTime;
                yield return null;
            }
            time -= frameTime;
            // ���� �ִϸ��̼� ��������Ʈ�� �̵�.
            // ������ ��������Ʈ��� ��
            if (sprites.Length == number) break;
            yield return null;
        }

        PoolController.PushEffect(id, gameObject);
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        if (sprites == null) return;
        StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
