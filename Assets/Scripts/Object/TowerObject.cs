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

        gameObject.name = id.ToString();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        rangeUI.transform.localPosition = range.transform.localPosition = Vector3.zero;

        return true;
    }

    private void Update()
    {
        // IDLE������ ��(Ư���� ����� ������ ���� ��) ���� �ٶ�.
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

        if (data.Buff(BuffType.GOLDMINE) != 0)
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
        // ���� �տ� �ִ� ���ֿ� ���� �¿����
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
            // ����: 100, ����: 10
            if (enemy.Data.WeakElement() == data.element) prior = 100;
            else if (enemy.Data.StrongElement() != data.element) prior = 10;

            // ���� ������ �� ���� �켱����
            // �� ������ ���ֱ��� ���� �켱������ ������ ��ġ�� ����.
            prior += enemy.Order / 10000f;
        }
        else if (priority == AttackPriority.DEBUFF)
        {
            foreach (DebuffType type in data.DebuffTypes)
            {
                int remainTime = enemy.DebuffRemainTime(type);
                float value = enemy.DebuffValue(type);

                // ���� �ð��� ���� ����
                // ���� ������ �� ���� ����� �� ��� �� ���� �켱����
                prior += (5 - remainTime);
                prior += data.Debuff(type) - value;
            }

            // ���� ������ �� ���� �켱����
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
        float delay = 1 / Stat(TowerStatType.ATTACKSPEED); // ���ݼӵ� ������

        while (enemies.Count > 0)
        {
            Animate(AnimationType.ATTACK, false);

            float time = 0;

            // attackTime ��ŭ �ݺ�
            for (int i = 0; i < data.attackTime.Length; i++)
            {
                float progressTime = data.attackTime[i];
                // ������ ����Ǵ� �������� ���
                while (time < progressTime)
                {
                    if (GameController.Instance.Paused)
                    {
                        yield return null;
                        continue;
                    }
                    time += Time.deltaTime;
                    yield return null;
                }

                // �� �� ���ݰ��� �Լ� ���� ����.

                int targetAmount = (int)data.Stat(TowerStatType.MULTISHOT);
                if (targetAmount == 0) targetAmount = 1;
                List<EnemyObject> target = enemies.Get(targetAmount);
                if (target == null || enemies.Count == 0) continue;

                // ����ü �߻� 
                if (TowerManager.Projs.ContainsKey(id))
                {
                    // ���� �߻�
                    for (int j = 0; j < target.Count; j++)
                    {
                        Vector3 start = transform.position;
                        Vector3 end = target[j].transform.position;

                        if (data.attackType == AttackType.POINT)
                            start = end;

                        PoolController.Pop(id, start, end);
                    }

                    progressTime += data.projAttackTime;
                    // ����ü�� ������ ������ �ð� ���.
                    while (time < progressTime)
                    {
                        if (GameController.Instance.Paused)
                        {
                            yield return null;
                            continue;
                        }
                        time += Time.deltaTime;
                        yield return null;
                    }
                }

                Effect(target);

                // ������� �켱������ ���ݽø��� ���ŵǾ�� ��.
                // ���� ��Ͽ��� ����, ���� ���� ����ִٸ� �߰��Ͽ� �켱���� ������.
                if (data.HasDebuff)
                {
                    // ���� ���� �ϳ��� ���� �� �̾ ����
                    enemies.Dequeue();
                    for (int j = 1; j < target.Count; j++)
                        enemies.Remove(target[j]);
                }

                SoundController.PlayAudio(id);
                if (data.Stat(TowerStatType.SPLASH) != 0)
                {
                    for (int j = 0; j < target.Count; j++)
                    {
                        SplashPoint point = TowerController.Instance.PopSplash();
                        point.transform.position = target[j].transform.position;
                        point.SetData(data);
                    }
                }
                else EnemyController.Instance.EnemyAttacked(target, data);

                // ����ִ� ������ �߰������� �켱���� ������
                if (data.HasDebuff)
                {
                    for (int j = 0; j < target.Count; j++)
                    {
                        // ����ִ� ��쿡�� �߰�
                        if (target[j].gameObject.activeSelf)
                        {
                            enemies.Enqueue(target[j], GetPriority(target[j]));
                        }
                    }
                }
            }

            while (time < delay)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                time += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        attackCoroutine = null;
    }

    public void Effect(List<EnemyObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            // Ÿ���� �׾��� ��쿡�� ���÷��� �����̶�� ����Ʈ�� ���ƾ� ��.
            // �ݴ�� Ÿ���� ���� ���÷��� ������ �ƴ϶�� ����Ʈ�� ���� �ʿ䰡 ����.
            if (targets[i].gameObject.activeSelf == false
                && data.Stat(TowerStatType.SPLASH) == 0)
                continue;

            GameObject effect = PoolController.PopEffect(data.id);

            if (effect != null)
            {
                effect.transform.parent = null;
                effect.transform.position = targets[i].transform.position;
            }
        }
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
            int value = (int)data.Buff(BuffType.GOLDMINE);
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
        // ������ ��� ������ȯ�� �����Ƿ� �̸� ������ȯ�� �ص�.
        if (curAnim == AnimationType.ATTACK) LookAtEnemy();

        int number = 0;
        float time = 0;
        float frameTime = data.spf;
        while (true)
        {
            // ��������Ʈ ����
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
            // ���� �ִϸ��̼� ��������Ʈ�� �̵�.
            // ������ ��������Ʈ�� �� loop��� ó������.
            number++;
            if (data.animation[anim].Length == number)
            {
                if (loop == false) break;
                number = 0;
            }
        }

        // ���� �ִϸ��̼��� ���� �ڿ��� �ڵ����� IDLE��
        if (anim == AnimationType.ATTACK) Animate(AnimationType.IDLE, true);
    }
}
