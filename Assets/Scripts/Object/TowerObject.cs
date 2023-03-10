using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : Poolable
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    public Vector3 Pos { get; private set; }
    public Tower Data { get { return data; } }
    private Tower data;

    private AnimationType curAnim;
    private IEnumerator animCoroutine;
    private IEnumerator attackCoroutine;
    private IEnumerator miningCoroutine;
    private PriorityQueue<EnemyObject> enemies;

    private AttackPriority priority;
    public AttackPriority Priority { get { return priority; } }


    public override bool MakePrefab(int id)
    {
        this.id = id;
        Tower data = TowerManager.GetTower(id);
        if (data == null) return false;

        amount = 2;

        gameObject.name = data.name;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        rangeUI.transform.localPosition = range.transform.localPosition = Vector3.zero;

        return true;
    }

    private void Update()
    {
        // IDLE상태일 때(특수한 모션을 취하지 않을 때) 몹을 바라봄.
        if (curAnim == AnimationType.IDLE) LookAtEnemy();
    }

    #region Build
    public void Build(Vector3 pos)
    {
        Pos = pos;

        Tower data = TowerManager.GetTower(id);
        this.data = new Tower(data);

        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;

        UpdateDistnace();

        rangeUI.SetActive(false);

        enemies = new PriorityQueue<EnemyObject>();

        priority = AttackPriority.FIRST;

        Animate(AnimationType.IDLE, true);
        attackCoroutine = null;
        miningCoroutine = null;

        if (data.Stat(TowerStatType.GOLDMINE) != 0)
        {
            miningCoroutine = Mining();
            StartCoroutine(miningCoroutine);
        }
    }

    public void SelectTower(bool b)
    {
        rangeUI.SetActive(b);
    }

    public void RemoveTower()
    {
        enemies = null;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
        }
    }
    #endregion

    #region Update Info
    public void UpdateLanguage(LanguageType lang)
    {
        data.UpdateName(Translator.GetLanguage(id), lang);
    }

    public void UpdateDistnace()
    {
        rangeUI.transform.localScale = range.transform.localScale = Vector3.one * (1 + data.Stat(TowerStatType.DISTANCE) * 2);
    }

    private void LookAtEnemy()
    {
        // 가장 앞에 있는 유닛에 따라 좌우반전
        if (enemies.Count <= 0) return;

        float enemyX = enemies.Get().transform.position.x;

        if (enemyX < transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    private float Stat(TowerStatType type)
    {
        float value = data.Stat(type);

        value += BonusStat(type);

        return value;
    }

    public float BonusStat(TowerStatType type)
    {
        float value;
        int stat = 0;

        if (type == TowerStatType.DAMAGE && PlayerController.Instance.Type == CharacterType.POWER)
            stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);

        else if (type == TowerStatType.ATTACKSPEED && PlayerController.Instance.Type == CharacterType.ATTACKSPEED)
            stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);

        stat += PlayerController.Instance.BonusElement(data.element);

        float percent = stat / 100f;
        value = data.Stat(type) * percent;

        return value;
    }

    public void ChangePriority(AttackPriority type)
    {
        priority = type;

        PriorityQueue<EnemyObject> tmp = new PriorityQueue<EnemyObject>();
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyObject enemy = enemies.Dequeue();
            tmp.Enqueue(enemy, GetPriority(enemy));
        }

        enemies = null;
        enemies = tmp;
    }

    public float GetPriority(EnemyObject enemy)
    {
        float prior = 0;
        if (priority == AttackPriority.FIRST)
            prior = enemy.Order;
        else if (priority == AttackPriority.LAST)
            prior = enemy.Order * -1;
        else if (priority == AttackPriority.STRONG)
            prior = enemy.Hp;
        else if (priority == AttackPriority.WEAK)
            prior = enemy.Hp * -1;
        else if (priority == AttackPriority.ELEMENT)
        {
            // 강속: 100, 동속: 10
            if (enemy.Data.WeakElement() == data.element) prior = 100;
            else if (enemy.Data.StrongElement() != data.element) prior = 10;

            // 앞의 유닛이 더 높은 우선순위
            // 만 마리의 유닛까지 앞의 우선순위에 영향을 끼치지 못함.
            prior += enemy.Order / 10000f;
        }
        else if (priority == AttackPriority.DEBUFF)
        {
            // 기본적으로 디버프 미부여 시 같은 우선순위 부여.
            if (data.Stat(TowerStatType.SLOW) != 0)
            {
                if (enemy.SlowAmount == 0) prior += 100;
                else prior += (data.Stat(TowerStatType.SLOW) - enemy.SlowAmount);
            }
            if (data.Stat(TowerStatType.DOTDAMAGE) != 0)
            {
                int remainTime = enemy.RemainDotDmaage(data.id);
                prior += (5 - remainTime) * 20;
            }

            // 앞의 유닛이 더 높은 우선순위
            prior += enemy.Order / 10000f;
        }

        return prior;
    }
    #endregion

    #region Activate
    public void AddEnemy(EnemyObject enemy)
    {
        enemies.Enqueue(enemy, GetPriority(enemy));
        if (attackCoroutine == null)
        {
            attackCoroutine = Attack();
            StartCoroutine(Attack());
        }
    }
    public void RemoveEnemy(EnemyObject enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    private IEnumerator Attack()
    {
        while (enemies.Count > 0)
        {
            Animate(AnimationType.ATTACK, false);
            int targetAmount = (int)data.Stat(TowerStatType.MULTISHOT);
            if (targetAmount == 0) targetAmount = 1;
            List<EnemyObject> target = enemies.Get(targetAmount);
            // 전부 목록에서 제거
            if (data.hasDebuff)
            {
                enemies.Dequeue();
                for (int i = 1; i < target.Count; i++)
                    enemies.Remove(target[i]);
            }

            SoundController.PlayAudio(id);
            if (data.Stat(TowerStatType.SPLASH) != 0)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    SplashPoint point = TowerController.Instance.PopSplash();
                    point.transform.position = target[i].transform.position;
                    point.SetData(data);
                }
            }
            else EnemyController.Instance.EnemyAttacked(target, data);

            if (data.hasDebuff)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    // 살아있는 경우에만 추가
                    if (target[i].gameObject.activeSelf)
                    {
                        enemies.Enqueue(target[i], GetPriority(target[i]));
                    }
                }
            }

            float delayTime = 0;
            float delay = 1 / Stat(TowerStatType.ATTACKSPEED);
            while (delayTime < delay)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                delayTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        attackCoroutine = null;
    }

    private IEnumerator Mining()
    {
        while (true)
        {
            float delayTime = 0;
            float delay = 1 / Stat(TowerStatType.ATTACKSPEED);
            while (delayTime < delay)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                delayTime += Time.deltaTime;
                yield return null;
            }
            int value = (int)data.Stat(TowerStatType.GOLDMINE);
            PlayerController.Instance.Reward(0, value);
            yield return null;
        }
    }
    #endregion
    
    private void Animate(AnimationType anim, bool loop = false)
    {
        if (data.animation.ContainsKey(anim) == false) return;
        curAnim = anim;

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
        // 공격일 경우 방향전환이 없으므로 미리 방향전환을 해둠.
        if (curAnim == AnimationType.ATTACK) LookAtEnemy();

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

        // 공격 애니메이션이 끝난 뒤에는 자동으로 IDLE로
        if (anim == AnimationType.ATTACK) Animate(AnimationType.IDLE, true);
    }
}
