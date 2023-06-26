using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyObject : Poolable
{
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject hpGage;
    private SpriteRenderer spriteRenderer;
    private Material material;

    public Enemy Data { get { return data; } }
    private Enemy data;

    private List<Vector3> road;
    private float maxHp;
    private float hp;
    private float speed;
    private int destRoad;
    public int Order { get; private set; }
    public float Hp { get { return hp; } }

    private IEnumerator animCoroutine;
    private IEnumerator moveCoroutine;
    private IEnumerator effectCoroutine;

    private Dictionary<DebuffType, EnemyDebuff> debuffs;

    private class EnemyDebuff
    {
        public IEnumerator coroutine;
        public float value;
        public int time;
    }

    #region Init

    public override bool MakePrefab(int id)
    {
        this.id = id;
        Enemy data = EnemyManager.GetEnemy(id);
        if (data == null) return false;

        amount = 2;

        this.data = new Enemy(data);
        gameObject.name = id.ToString();
        tag = "Enemy";

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Enemy";
        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        material = spriteRenderer.material;

        hpBar.transform.localPosition = new Vector3(0, this.data.Height);

        debuffs = new Dictionary<DebuffType, EnemyDebuff>();
        return true;
    }

    public void Init(List<Vector3> road, int order, float hpDif, float speedDif)
    {
        Reset();
        this.road = road;

        hp = maxHp = data.hp * hpDif;
        UpdateHp();
        speed = data.speed * speedDif;

        transform.position = road[0];
        destRoad = 1;

        spriteRenderer.sortingOrder = Order = order;

        Animate(AnimationType.MOVE, true);
        moveCoroutine = Move();
        StartCoroutine(moveCoroutine);
    }

    private void Reset()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        foreach (var debuff in debuffs.Values)
        {
            StopCoroutine(debuff.coroutine);
        }
        debuffs.Clear();
        material.SetFloat("_FlashAmount", 0);
    }
    #endregion

    private IEnumerator Move()
    {
        while (destRoad < road.Count)
        {
            int curRoad = destRoad - 1;
            Vector3 v = Vector3.Normalize(road[destRoad] - road[curRoad]);
            bool destlargeX = road[destRoad].x > road[curRoad].x;
            bool destlargeY = road[destRoad].y > road[curRoad].y;

            // 상하이동의 경우 스프라이트 flip 진행 X
            if (road[destRoad].x != road[curRoad].x)
            {
                if (destlargeX) spriteRenderer.flipX = true;
                else spriteRenderer.flipX = false;
            }

            while (CompareVector(transform.position, road[destRoad], destlargeX, destlargeY))
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                float slowAmount = 0;
                if (debuffs.ContainsKey(DebuffType.SLOW)) slowAmount = debuffs[DebuffType.SLOW].value;
                Vector3 moveAmount = v * speed * Time.deltaTime * (1 - slowAmount);
                transform.position += moveAmount;
                yield return null;
            }
            transform.position = road[destRoad];

            destRoad++;
        }
        EnemyController.Instance.EnemyArrive(this);
    }

    // first, second, secondLargeX, secondLargeY
    private bool CompareVector(Vector3 first, Vector3 second, bool slx, bool sly)
    {
        return (((slx && transform.position.x <= road[destRoad].x) || (!slx && transform.position.x >= road[destRoad].x))
            && ((sly && transform.position.y <= road[destRoad].y) || (!sly && transform.position.y >= road[destRoad].y)));
    }

    #region Damaged
    public void Attacked(Tower tower, float dmg)
    {
        Element element = tower.element;

        if (dmg != 0)
        {
            Damaged(element, dmg, tower.effectColor);
        }

        if (hp <= 0) return;
        // 디버프 추가
        if (tower.DebuffTypes != null)
        {
            for (int i = 0; i < tower.DebuffTypes.Length; i++)
            {
                DebuffType type = tower.DebuffTypes[i];
                float value = tower.Debuff(type);

                if (debuffs.ContainsKey(type) == false)
                {
                    EnemyDebuff debuff = new EnemyDebuff
                    {
                        coroutine = Debuff(type, element),
                        value = value,
                        time = 5
                    };
                    debuffs.Add(type, debuff);
                    StartCoroutine(debuffs[type].coroutine);
                }
                else
                {
                    if (debuffs[type].value < value) debuffs[type].value = value;
                    debuffs[type].time = 5;
                }
            }
        }
    }

    public void Damaged(Element element, float dmg, Color damagedColor)
    {
        if (element == data.WeakElement()) dmg *= 1.5f;
        else if (element == data.StrongElement()) dmg *= 0.5f;

        UIController.Instance.EnemyDamaged(transform.position, dmg);
        Effect(damagedColor);

        hp -= dmg;
        UpdateHp();
#if UNITY_EDITOR
        //Debug.Log($"[SYSTEM] {name} Damaged {dmg} | HP: {hp}");
#endif

        if (hp <= 0)
        {
            Reset();
            PlayerController.Instance.Reward(data.exp, data.money);
            EnemyController.Instance.RemoveEnemy(this);
        }
    }

    private void UpdateHp()
    {
        if (hp < 0) hp = 0;
        hpGage.transform.localScale = new Vector3(hp / maxHp, 1);
    }
    #endregion

    #region Debuff
    public IEnumerator Debuff(DebuffType type, Element element)
    {
        float time = 0;
        while (hp > 0 && debuffs[type].time > 0)
        {
            while (time < 1)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                time += Time.deltaTime;
                yield return null;
            }
            time -= 1;
            debuffs[type].time -= 1;

            Color c = Color.white;
            switch (type)
            {
                case DebuffType.SLOW:
                    c = Color.black;
                    break;
                case DebuffType.BURN:
                    c = new Color(1, 0.5f, 0);
                    break;
                case DebuffType.POISON:
                    c = Color.magenta;
                    break;
                case DebuffType.BLEED:
                    c = Color.red;
                    break;
            }

            float value = (type >= DebuffType.BURN) ? debuffs[type].value : 0;
            Damaged(element, value, c);
        }

        debuffs.Remove(type);
    }

    public int DebuffRemainTime(DebuffType type)
    {
        if (debuffs.ContainsKey(type)) return debuffs[type].time;

        return 0;
    }

    public float DebuffValue(DebuffType type)
    {
        if (debuffs.ContainsKey(type)) return debuffs[type].value;

        return 0;
    }
    #endregion

    #region Animation
    private void Animate(AnimationType anim, bool loop = false)
    {
        if (data.animation.ContainsKey(anim) == false) return;
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        animCoroutine = Animation(anim, loop);
        StartCoroutine(animCoroutine);
    }

    private IEnumerator Animation(AnimationType anim, bool loop)
    {
        int number = 0;
        float time = 0;
        // 한 프레임당 100ms
        float frameTime = 0.1f;
        while (true)
        {
            spriteRenderer.sprite = data.animation[anim][number];
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
            // 마지막 스프라이트일 때 loop라면 처음으로.
            number++;
            if (data.animation[anim].Length == number)
            {
                if (loop == false) break;
                number = 0;
            }
        }
    }

    #region Effect
    public void Effect(Color color)
    {
        material.SetColor("Flash Color", color);

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }

        effectCoroutine = Effect();
        StartCoroutine(effectCoroutine);
    }

    private IEnumerator Effect()
    {
        float unit = 4f;
        float time = 1 / unit;
        while (time > 0)
        {
            material.SetFloat("_FlashAmount", time * unit);
            time -= Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_FlashAmount", 0);
    }

    #endregion
    #endregion
}
