using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : Poolable
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;
    // ��� ����ü�� �ӵ��� �����ϰ� ����
    public static float flyingTime { get; private set; } = 0.15f;

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
        // �� �����Ӵ� 30ms
        float frameTime = 0.03f;
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
            // ������ ��������Ʈ��� ó������.
            if (sprites.Length == number) number = 0;
        }
    }

    IEnumerator Shooting(Vector2 start, Vector2 end)
    {
        Vector3 dir = end - start;
        transform.position = start;
        float time = 0;

        /* ���⿡ ���� ����ü�� ȸ����Ŵ.
         * tan(rad) = y / x = dir.y / dir.x
         * rad = atan(dir.y / dir.x)
         */

        // ���������� �������� �� ȸ��
        transform.rotation = Quaternion.identity;

        float degree = Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg;
        transform.Rotate(new Vector3(0, 0, degree));

        while (time < flyingTime)
        {
            if (GameController.Instance.Paused)
            {
                yield return null;
                continue;
            }
            time += Time.deltaTime;
            transform.position += dir / flyingTime * Time.deltaTime;
            yield return null;
        }

        PoolController.PushProj(id, gameObject);
    }

    private void OnEnable()
    {
        if (sprites == null) return;

        // x,y;x,y ����
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
