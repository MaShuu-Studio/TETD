using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : Poolable
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;

    public override bool MakePrefab(int id)
    {
        this.id = id;
        amount = 5;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (TowerManager.Projs.ContainsKey(id))
        {
            sprites = TowerManager.Projs[id];
            return true;
        }

        return false;
    }

    public void SetSprite(Sprite[] sprites)
    {
        this.sprites = sprites;
    }

    IEnumerator Animation()
    {
        int number = 0;
        float time = 0;
        // 한 프레임당 40ms
        float frameTime = 0.04f;
        while (true)
        {
            // 스프라이트 변경
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
            // 다음 애니메이션 스프라이트로 이동.
            // 마지막 스프라이트라면 처음으로.
            if (sprites.Length == number) number = 0;
        }
    }

    IEnumerator Shooting(Vector2 start, Vector2 end)
    {
        Vector3 dir = end - start;
        transform.position = start;
        float time = 0;
        float total = 0.25f;

        while (time < total)
        {
            if (GameController.Instance.Paused)
            {
                yield return null;
                continue;
            }
            time += Time.deltaTime;
            transform.position += dir / total * Time.deltaTime;
            yield return null;
        }

        PoolController.PushProj(id, gameObject);
    }

    private void OnEnable()
    {
        if (sprites == null) return;

        // x,y;x,y 형태
        string[] se = gameObject.name.Split(";");

        Vector2[] pos = new Vector2[2]; // start, end

        for (int i = 0; i < 2; i++)
        {
            string[] xy = se[i].Split(",");
            float x, y;
            float.TryParse(xy[0], out x);
            float.TryParse(xy[1], out y);

            pos[i] = new Vector2(x, y);
        }

        StartCoroutine(Animation());
        StartCoroutine(Shooting(pos[0], pos[1]));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
