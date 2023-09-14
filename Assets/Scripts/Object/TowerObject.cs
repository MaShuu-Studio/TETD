using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class TowerObject : Poolable
{
    [SerializeField] private GameObject rangeUI;
    [SerializeField] private TowerAttackArea range;
    private SpriteRenderer spriteRenderer;

    public Vector3 Pos { get; private set; }
    public Tower Data { get { return data; } }
    private Tower data;

    private float OriginAttackSpeed;

    private AnimationType curAnim;
    private IEnumerator animCoroutine;
    private IEnumerator activateCoroutine;
    private IEnumerator buffCoroutine;

    private PriorityQueue<EnemyObject> enemies;
    private PriorityQueue<TowerObject> towers;

    public AttackPriority Priority { get { return priority; } }
    private AttackPriority priority;
    public Dictionary<AbilityType, TowerBuff> Buffs { get { return buffs; } }
    private Dictionary<AbilityType, TowerBuff> buffs;

    private enum ActiveType { ONLYATTACK = 0, ONLYBUFF, ATTACKWITHBUFF, }
    private ActiveType activeType;

    public class TowerBuff
    {
        public IEnumerator coroutine;
        public float value;
        public float time;
    }

    public override bool MakePrefab(int id)
    {
        this.id = id;
        Tower data = TowerManager.GetTower(id);
        if (data == null) return false;

        amount = 5;

        gameObject.name = id.ToString();

        OriginAttackSpeed = data.Stat(TowerStatType.ATTACKSPEED);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        spriteRenderer.sprite = SpriteManager.GetSprite(id);

        // ������ ������ ��� ������ �����ε� Range�� �۵������� 
        // �ݱ��� ������ ������ ��� (������ 1���̰� �װ� �ݱ��� ���)
        // ������ ������ ������ ����.
        bool hasBuff = data.HasBuff;
        if (data.HasBuff)
        {
            bool hasGoldMine = false;
            int buffCount = 0;
            for (int i = 0; i < data.AbilityTypes.Length; i++)
            {
                if (data.AbilityTypes[i] == AbilityType.GOLDMINE) hasGoldMine = true;
                if (Tower.IsBuff(data.AbilityTypes[i])) buffCount++;

                if (buffCount > 1) break;
            }

            if (buffCount == 1 && hasGoldMine) hasBuff = false;
        }

        if (hasBuff == false) activeType = ActiveType.ONLYATTACK;
        else if (data.Stat(TowerStatType.DAMAGE) == 0) activeType = ActiveType.ONLYBUFF;
        else activeType = ActiveType.ATTACKWITHBUFF;

        range.Init(this, hasBuff);
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
        towers = new PriorityQueue<TowerObject>();

        priority = AttackPriority.FIRST;

        Animate(AnimationType.IDLE, true);
        activateCoroutine = null;

        buffs = new Dictionary<AbilityType, TowerBuff>();

        // �ݱ��� ��� �ڹ��� �� �ڵ� �۵��̹Ƿ� �ٷ� �߰�
        if (data.Ability(AbilityType.GOLDMINE) != 0)
        {
            UpdateBuff(AbilityType.GOLDMINE, data.Ability(AbilityType.GOLDMINE), 99);
        }
    }

    public void SelectTower(bool b)
    {
        rangeUI.SetActive(b);
    }

    public void Reset()
    {
        enemies = null;
        towers = null;
        if (activateCoroutine != null)
        {
            StopCoroutine(activateCoroutine);
            activateCoroutine = null;
        }
        foreach (var buff in buffs.Values)
        {
            StopCoroutine(buff.coroutine);
        }
        buffs.Clear();
        TowerController.Instance.RemoveTowerObject(this);
    }
    #endregion

    #region Update Info
    public void UpdateLanguage(LanguageType lang)
    {
        data.UpdateName(Translator.GetLanguage(id));
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

    public float Stat(TowerStatType type)
    {
        float value = data.Stat(type);

        value += BonusStat(type);

        return value;
    }

    private float BonusStat(TowerStatType type)
    {
        float value = 0;
        int stat = 0;

        if (type == TowerStatType.DAMAGE)
        {
            stat += PlayerController.Instance.BonusElement(data.element, Character.ElementStatType.DMG);

            // ������ ���� �߰� ����
            // ������ �ƴ� ��ġ�� �����ϱ� ������ value�� ���� ����
            // ���� ���� ������ �����ϰ� �Ǹ� stat ���� ������. ��Ȳ�� ���� �հ��, ����� ����
            if (buffs.ContainsKey(AbilityType.DMG))
                value += buffs[AbilityType.DMG].value;
        }

        else if (type == TowerStatType.ATTACKSPEED)
        {
            stat += PlayerController.Instance.BonusElement(data.element, Character.ElementStatType.ATTACKSPEED);

            if (buffs.ContainsKey(AbilityType.ATKSPD))
                value += buffs[AbilityType.ATKSPD].value;
        }

        // �������� ������ ���ʽ� ������ �ۼ�Ƽ���� ����
        float percent = stat / 100f;
        value += data.Stat(type) * percent;

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

    #region Enemy
    public void AddEnemy(EnemyObject enemy)
    {
        if (activeType == ActiveType.ONLYBUFF) return;

        enemies.Enqueue(enemy, GetPriority(enemy));
        if (activateCoroutine == null)
        {
            activateCoroutine = Activate();
            StartCoroutine(activateCoroutine);
        }
    }
    public void RemoveEnemy(EnemyObject enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
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
            if (data.HasDebuff)
                foreach (AbilityType type in data.AbilityTypes)
                {
                    if (Tower.IsDebuff(type) == false) continue;
                    int remainTime = enemy.DebuffRemainTime(type);
                    float value = enemy.DebuffValue(type);

                    // ���� �ð��� ���� ����
                    // ���� ������ �� ���� ����� �� ��� �� ���� �켱����
                    prior += (5 - remainTime);
                    prior += data.Ability(type) - value;
                }

            // ���� ������ �� ���� �켱����
            prior += enemy.Order / 10000f;
        }

        return prior;
    }
    #endregion

    #region Buff
    public void AddTower(TowerObject tower)
    {
        towers.Enqueue(tower, GetBuffPriority(tower));

        if (activeType == ActiveType.ATTACKWITHBUFF)
        {
            if (buffCoroutine == null)
            {
                buffCoroutine = ActiveWithBuff();
                StartCoroutine(buffCoroutine);
            }
        }
        else if (activateCoroutine == null)
        {
            activateCoroutine = Activate();
            StartCoroutine(activateCoroutine);
        }
    }

    public void RemoveTower(TowerObject tower)
    {
        if (data.HasBuff == false) return;

        if (towers.Contains(tower))
            towers.Remove(tower);
    }

    public float GetBuffPriority(TowerObject tower)
    {
        float prior = 0;
        foreach (AbilityType type in data.AbilityTypes)
        {
            if (Tower.IsBuff(type) == false) continue;
            if (type == AbilityType.GOLDMINE) continue;

            float remainTime = 0;
            float value = 0;
            // �ش� Ÿ���� ������ ���� ���� ��� �߰� ����
            if (tower.Buffs.ContainsKey(type))
            {
                remainTime = tower.Buffs[type].time;
                value = tower.Buffs[type].value;
            }

            // ���� �ð��� ���� ����
            // ���� ������ �� ���� ���� �� ��� �� ���� �켱����
            prior += (5 - remainTime);
            prior += data.Ability(type) - value;
        }

        return prior;
    }

    public void UpdateBuff(AbilityType type, float value, float time)
    {
        // �̹� �ִ� ������ ��� ������Ʈ
        if (buffs.ContainsKey(type))
        {
            // �� ū ���̳� �ð��� ��� ������Ʈ
            if (buffs[type].value < value) buffs[type].value = value;
            if (buffs[type].time < time) buffs[type].time = time;
        }
        // ���� ��� ���Ӱ� �߰�
        else
        {
            TowerBuff buff = new TowerBuff()
            {
                coroutine = Buff(type),
                value = value,
                time = time
            };
            buffs.Add(type, buff);
            StartCoroutine(buffs[type].coroutine);
        }
    }

    private IEnumerator Buff(AbilityType type)
    {
        while (buffs[type].time > 0)
        {
            // �ݱ������� ��� ���� �ֱ�(���� �ӵ�)���� ����ؼ� ��� ȹ��
            // ������ ������� �����Ƿ� �ð� ���� X
            if (type == AbilityType.GOLDMINE)
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
                PlayerController.Instance.Reward(0, (int)buffs[type].value);
            }
            // �� ���� ��� ������ ���� �ɷ�ġ ȹ�� (Bonus Stat���� �۵�)
            // ����ؼ� �ð��� ���ŵǸ� 0�� �� ��� ���� ����
            else
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                buffs[type].time -= Time.deltaTime;
                yield return null;
            }
            yield return null;
        }

        buffs.Remove(type);
    }
    #endregion
    #endregion

    #region Activate
    private IEnumerator Activate()
    {
        while (true)
        {
            if (activeType != ActiveType.ONLYBUFF && enemies.Count <= 0) break;
            if (activeType == ActiveType.ONLYBUFF && towers.Count <= 0) break;

            // Stat�Լ� ��ü�� ������ ����ǹǷ� ���� �����ص�
            // ��� ���� = ���� ���ݼӵ� / �⺻����
            // ���ݼӵ� ������

            float curASPD = Stat(TowerStatType.ATTACKSPEED);
            float speedRatio = curASPD / OriginAttackSpeed;
            float delay = 1 / curASPD;
            Animate(AnimationType.ATTACK, false, speedRatio);

            float time = 0;

            // attackTime ��ŭ �ݺ�
            for (int i = 0; i < data.attackTime.Length; i++)
            {
                // ����
                float progressTime = data.attackTime[i] / speedRatio;
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

                // ������ �Ŀ��� �۵�
                if (activeType != ActiveType.ONLYBUFF && enemies.Count > 0) 
                    StartCoroutine(Attack(speedRatio));

                if (activeType == ActiveType.ONLYBUFF && towers.Count > 0) 
                    GiveBuff();

                yield return null;
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
        activateCoroutine = null;
    }

    // ���� ���� �ڷ�ƾ
    public IEnumerator Attack(float speedRatio)
    {
        float time = 0;

        // �� �� ���ݰ��� �Լ� ���� ����.
        int targetAmount = (int)data.Ability(AbilityType.MULTISHOT);
        if (targetAmount == 0) targetAmount = 1;
        List<EnemyObject> target = enemies.Get(targetAmount);
        if (target != null && enemies.Count > 0)
        {
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

                float progressTime = data.projAttackTime / speedRatio;

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
            if (data.Ability(AbilityType.SPLASH) != 0)
            {
                for (int j = 0; j < target.Count; j++)
                {
                    SplashPoint point = TowerController.Instance.PopSplashPoint();
                    point.transform.position = target[j].transform.position;
                    point.SetData(data, Stat(TowerStatType.DAMAGE));
                }
            }
            else EnemyController.Instance.EnemyAttacked(target, data, Stat(TowerStatType.DAMAGE));

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
    }

    // ���� ���� �ڷ�ƾ.
    private IEnumerator ActiveWithBuff()
    {
        while (true)
        {
            if (towers.Count <= 0) break;

            // ������ ������ ���� �ϴ� ���
            // �ִϸ��̼��� ���ݿ� ���ߵ� ������ ���� �۵��ϰ� ��.
            // ���� ���Ӱ��� ������ ���� ����� �۵��ϰ� ��.
            // ����� �ӽ÷� 3�ʸ��� �ڵ����� �۵��ϰ� ��.

            float curASPD = Stat(TowerStatType.ATTACKSPEED);
            float speedRatio = curASPD / OriginAttackSpeed;
            float delay = 1 / curASPD;
            Animate(AnimationType.ATTACK, false, speedRatio);

            float time = 0;

            while (time < 3)
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                time += Time.deltaTime;
                yield return null;
            }

            if (towers.Count > 0)
                GiveBuff();

            yield return null;
        }
        buffCoroutine = null;
    }

    // ���� �ο� �Լ�
    private void GiveBuff()
    {
        int targetAmount = (int)data.Ability(AbilityType.MULTISHOT);
        if (targetAmount == 0) targetAmount = 1;
        List<TowerObject> target = towers.Get(targetAmount);
        if (target != null && towers.Count > 0)
        {
            Effect(target);
            for (int i = 0; i < target.Count; i++)
            {
                foreach (AbilityType type in data.AbilityTypes)
                {
                    if (Tower.IsBuff(type) == false) continue;
                    if (type == AbilityType.GOLDMINE) continue;
                    target[i].UpdateBuff(type, data.Ability(type), 5);
                }
            }

            // ������ �켱������ ���� �� �� �������־����.
            // ���� ��Ͽ��� ���� �� ���߰�.
            TowerObject[] tmp = new TowerObject[towers.Count];
            towers.CopyTo(tmp);
            towers.Clear();

            for (int i = 0; i < tmp.Length; i++)
            {
                towers.Enqueue(tmp[i], GetBuffPriority(tmp[i]));
            }
        }
    }

    public void Effect(List<EnemyObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            // Ÿ���� �׾��� ��쿡�� ���÷��� �����̶�� ����Ʈ�� ���ƾ� ��.
            // �ݴ�� Ÿ���� ���� ���÷��� ������ �ƴ϶�� ����Ʈ�� ���� �ʿ䰡 ����.
            if (targets[i].gameObject.activeSelf == false
                && data.Ability(AbilityType.SPLASH) == 0)
                continue;

            GameObject effect = PoolController.PopEffect(data.id);

            if (effect != null)
            {
                effect.transform.parent = null;
                effect.transform.position = targets[i].transform.position;
            }
        }
    }

    // ���� ����Ʈ
    public void Effect(List<TowerObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            GameObject effect = PoolController.PopBuffEffect(data.id);

            if (effect != null)
            {
                effect.transform.parent = null;
                effect.transform.position = targets[i].transform.position;
            }
        }
    }
    #endregion

    private void Animate(AnimationType anim, bool loop, float speedRatio = 1)
    {
        if (data.animation.ContainsKey(anim) == false) return;
        curAnim = anim;

        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        animCoroutine = Animation(anim, loop, speedRatio);
        StartCoroutine(animCoroutine);
    }

    private IEnumerator Animation(AnimationType anim, bool loop, float speedRatio)
    {
        // ������ ��� ������ȯ�� �����Ƿ� �̸� ������ȯ�� �ص�.
        if (curAnim == AnimationType.ATTACK) LookAtEnemy();

        int number = 0;
        float time = 0;
        float frameTime = data.spf / speedRatio;
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
