using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyObject : Poolable
{
    [SerializeField] private GameObject hpGage;
    private SpriteRenderer spriteRenderer;

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
    public float SlowAmount { get { return slowAmount; } }
    private float slowAmount;
    private Dictionary<int, IEnumerator> dotCoroutines;
    private Dictionary<int, int> dotTime;

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

        slowAmount = 0;
        dotCoroutines = new Dictionary<int, IEnumerator>();
        dotTime = new Dictionary<int, int>();
        return true;
    }

    public void Init(List<Vector3> road, int order, float hpDif, float speedDif)
    {
        this.road = road;

        hp = maxHp = data.hp * hpDif;
        UpdateHp();
        speed = data.speed * speedDif;

        transform.position = road[0];
        destRoad = 1;

        spriteRenderer.sortingOrder = Order = order;

        Animate(AnimationType.IDLE, true);
        moveCoroutine = Move();
        StartCoroutine(moveCoroutine);
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
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                Vector3 moveAmount = v * speed * Time.deltaTime * (1 - slowAmount);
                transform.position += moveAmount;
                yield return null;
            }
            transform.position = road[destRoad];

            destRoad++;
        }
        EnemyController.Instance.EnemyArrive(this);
    }

    public void Attacked(Tower tower)
    {
        Element element = tower.element;
        float dmg = tower.Stat(TowerStatType.DAMAGE);

        if (dmg != 0) Damaged(element, dmg);

        if (hp <= 0) return;
        // 공격력, 공격속도, 사거리 제외
        for (int i = 3; i < tower.StatTypes.Count; i++)
        {
            TowerStatType type = tower.StatTypes[i];
            float value = tower.Stat(type);

            if (type == TowerStatType.DOTDAMAGE)
            {
                if (dotCoroutines.ContainsKey(tower.id) == false)
                {
                    dotTime.Add(tower.id, 5);
                    dotCoroutines.Add(tower.id, DotDamage(tower.id, element, value));
                    StartCoroutine(dotCoroutines[tower.id]);
                }
                else dotTime[tower.id] = 5;
            }
            else if (type == TowerStatType.SLOW)
            {
                if (slowAmount * 100 < value)
                    slowAmount = value / 100;
            }
        }
    }

    public IEnumerator DotDamage(int id, Element element, float dmg)
    {
        float time = 0;
        while (hp > 0 && dotTime[id] > 0)
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
            dotTime[id] -= 1;
            Damaged(element, dmg);
        }

        dotCoroutines.Remove(id);
        dotTime.Remove(id);
    }

    public int RemainDotDmaage(int id)
    {
        if (dotTime.ContainsKey(id)) return dotTime[id];

        return 0;
    }

    public void Damaged(Element element, float dmg)
    {
        if (element == data.WeakElement()) dmg *= 1.5f;
        else if (element == data.StrongElement()) dmg *= 0.5f;

        UIController.Instance.EnemyDamaged(transform.position, dmg);

        hp -= dmg;
        UpdateHp();
#if UNITY_EDITOR
        //Debug.Log($"[SYSTEM] {name} Damaged {dmg} | HP: {hp}");
#endif

        if (hp <= 0)
        {
            StopCoroutine(moveCoroutine);
            foreach (var coroutine in dotCoroutines.Values)
            {
                StopCoroutine(coroutine);
            }
            moveCoroutine = null;
            dotCoroutines.Clear();
            dotTime.Clear();
            slowAmount = 0;

            PlayerController.Instance.Reward(data.exp, data.money);
            TowerController.Instance.RemoveEnemyObject(this);
            EnemyController.Instance.RemoveEnemy(this);
        }
    }

    private void UpdateHp()
    {
        if (hp < 0) hp = 0;
        hpGage.transform.localScale = new Vector3(hp / maxHp, 1);
    }

    // first, second, secondLargeX, secondLargeY
    private bool CompareVector(Vector3 first, Vector3 second, bool slx, bool sly)
    {
        return (((slx && transform.position.x <= road[destRoad].x) || (!slx && transform.position.x >= road[destRoad].x))
            && ((sly && transform.position.y <= road[destRoad].y) || (!sly && transform.position.y >= road[destRoad].y)));
    }

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
            // 스프라이트 변경
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
}
