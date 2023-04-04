using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

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

    IEnumerator Animation(float spf, bool loop = false)
    {
        int number = 0;
        float time = 0;
        float frameTime = spf;
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
            if (sprites.Length == number)
            {
                if (loop) number = 0;
                else break;
            }
            yield return null;
        }
    }

    IEnumerator Attack(float remainTime, Vector2 start, Vector2 end)
    {
        Vector3 dir = end - start;
        transform.position = start;

        if (start == end)
            dir = Vector3.zero;

        float time = 0;

        /* 방향에 따라 투사체를 회전시킴.
         * tan(rad) = y / x = dir.y / dir.x
         * rad = atan(dir.y / dir.x)
         */

        // 정방향으로 돌려놓은 뒤 회전
        transform.rotation = Quaternion.identity;

        if (dir != Vector3.zero)
        {
            float degree = Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg;
            transform.Rotate(new Vector3(0, 0, degree));
        }

        while (time < remainTime)
        {
            if (GameController.Instance.Paused)
            {
                yield return null;
                continue;
            }
            time += Time.deltaTime;
            if (dir != Vector3.zero)
                transform.position += dir / remainTime * Time.deltaTime;
            yield return null;
        }

        PoolController.PushProj(id, gameObject);
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        if (sprites == null) return;

        // loop;spf;remainTime;start;end 형태
        string[] se = gameObject.name.Split(";");
        float spf, remain;
        float.TryParse(se[1], out spf);
        float.TryParse(se[2], out remain);

        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        // value1: start pos
        // value2: end pos
        Vector2[] pos = new Vector2[2]; // start, end

        for (int i = 0; i < 2; i++)
        {
            string[] xy = se[i + 3].Split(",");
            float x, y;
            float.TryParse(xy[0], out x);
            float.TryParse(xy[1], out y);

            pos[i] = new Vector2(x, y);
        }

        start = pos[0];
        end = pos[1];

        bool loop = false;

        if (se[0] == "T") loop = true;

        StartCoroutine(Animation(spf, loop));
        StartCoroutine(Attack(remain, start, end));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
