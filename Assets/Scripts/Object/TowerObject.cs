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

    private PriorityQueue<EnemyObject> enemies;
    private PriorityQueue<TowerObject> towers;

    public AttackPriority Priority { get { return priority; } }
    private AttackPriority priority;
    public Dictionary<BuffType, TowerBuff> Buffs { get { return buffs; } }
    private Dictionary<BuffType, TowerBuff> buffs;

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

        // 버프를 가졌을 경우 버프의 범위로도 Range가 작동하지만 
        // 금광이 유일한 버프일 경우 (갯수가 1개이고 그게 금광일 경우)
        // 버프의 범위로 사용되지 않음.
        bool hasBuff =
            data.HasBuff &&
            !(data.BuffTypes.Length == 1 && data.BuffTypes[0] == BuffType.GOLDMINE);
        range.Init(this, hasBuff);
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
        towers = new PriorityQueue<TowerObject>();

        priority = AttackPriority.FIRST;

        Animate(AnimationType.IDLE, true);
        activateCoroutine = null;

        buffs = new Dictionary<BuffType, TowerBuff>();

        // 금광의 경우 자버프 겸 자동 작동이므로 바로 추가
        if (data.Buff(BuffType.GOLDMINE) != 0)
        {
            UpdateBuff(BuffType.GOLDMINE, data.Buff(BuffType.GOLDMINE), 99);
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
            if (PlayerController.Instance.Type == CharacterType.POWER)
                stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);
            // 버프로 스탯 추가 증가
            // 비율이 아닌 수치로 증가하기 때문에 value의 값을 변경
            // 만약 비율 증가로 변경하게 되면 stat 값을 조정함. 상황에 따라 합계산, 곱계산 적용
            if (buffs.ContainsKey(BuffType.DMG))
                value += buffs[BuffType.DMG].value;
        }

        else if (type == TowerStatType.ATTACKSPEED)
        {
            if (PlayerController.Instance.Type == CharacterType.ATTACKSPEED)
                stat = PlayerController.Instance.GetStat(CharacterStatType.ABILITY);

            if (buffs.ContainsKey(BuffType.ATKSPD))
                value += buffs[BuffType.ATKSPD].value;
        }

        stat += PlayerController.Instance.BonusElement(data.element);

        // 스탯으로 증가한 보너스 스탯은 퍼센티지로 증가
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
            // 강속: 100, 동속: 10
            if (enemy.Data.WeakElement() == data.element) prior = 100;
            else if (enemy.Data.StrongElement() != data.element) prior = 10;

            // 앞의 유닛이 더 높은 우선순위
            // 만 마리의 유닛까지 앞의 우선순위에 영향을 끼치지 못함.
            prior += enemy.Order / 10000f;
        }
        else if (priority == AttackPriority.DEBUFF)
        {
            foreach (DebuffType type in data.DebuffTypes)
            {
                int remainTime = enemy.DebuffRemainTime(type);
                float value = enemy.DebuffValue(type);

                // 남은 시간이 적을 수록
                // 기존 값보다 더 강한 디버프 일 경우 더 높은 우선순위
                prior += (5 - remainTime);
                prior += data.Debuff(type) - value;
            }

            // 앞의 유닛이 더 높은 우선순위
            prior += enemy.Order / 10000f;
        }

        return prior;
    }
    #endregion

    #region Buff
    public void AddTower(TowerObject tower)
    {
        towers.Enqueue(tower, GetPriority(tower));

        if (activateCoroutine == null)
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

    public float GetPriority(TowerObject tower)
    {
        float prior = 0;
        foreach (BuffType type in data.BuffTypes)
        {
            if (type == BuffType.GOLDMINE) continue;

            float remainTime = 0;
            float value = 0;
            // 해당 타워가 버프를 갖고 있을 경우 추가 조정
            if (tower.Buffs.ContainsKey(type))
            {
                remainTime = tower.Buffs[type].time;
                value = tower.Buffs[type].value;
            }

            // 남은 시간이 적을 수록
            // 기존 값보다 더 강한 버프 일 경우 더 높은 우선순위
            prior += (5 - remainTime);
            prior += data.Buff(type) - value;
        }

        return prior;
    }

    public void UpdateBuff(BuffType type, float value, float time)
    {
        // 이미 있는 버프일 경우 업데이트
        if (buffs.ContainsKey(type))
        {
            // 더 큰 값이나 시간일 경우 업데이트
            if (buffs[type].value < value) buffs[type].value = value;
            if (buffs[type].time < time) buffs[type].time = time;
        }
        // 없을 경우 새롭게 추가
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

    private IEnumerator Buff(BuffType type)
    {
        while (buffs[type].time > 0)
        {
            // 금광버프일 경우 일정 주기(공격 속도)마다 계속해서 골드 획득
            // 버프가 사라지지 않으므로 시간 갱신 X
            if (type == BuffType.GOLDMINE)
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
            // 그 외의 경우 버프에 따라 능력치 획득 (Bonus Stat에서 작동)
            // 계속해서 시간이 갱신되며 0이 될 경우 버프 삭제
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
            if (enemies.Count <= 0 && towers.Count <= 0) break;

            // Stat함수 자체가 연산이 진행되므로 값을 저장해둠
            // 배속 정도 = 현재 공격속도 / 기본공속
            // 공격속도 딜레이

            float curASPD = Stat(TowerStatType.ATTACKSPEED);
            float speedRatio = curASPD / OriginAttackSpeed;
            float delay = 1 / curASPD;
            Animate(AnimationType.ATTACK, false, speedRatio);

            float time = 0;

            // attackTime 만큼 반복
            for (int i = 0; i < data.attackTime.Length; i++)
            {
                // 선딜
                float progressTime = data.attackTime[i] / speedRatio;
                // 공격이 진행되는 구간까지 대기
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

                // 선딜이 후에는 작동
                if (enemies.Count > 0) StartCoroutine(Attack(speedRatio));
                if (towers.Count > 0) GiveBuff();

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

    // 공격 관련 코루틴
    public IEnumerator Attack(float speedRatio)
    {
        float time = 0;

        // 이 후 공격관련 함수 전부 진행.
        int targetAmount = (int)data.Stat(TowerStatType.MULTISHOT);
        if (targetAmount == 0) targetAmount = 1;
        List<EnemyObject> target = enemies.Get(targetAmount);
        if (target != null && enemies.Count > 0)
        {
            // 투사체 발사 
            if (TowerManager.Projs.ContainsKey(id))
            {
                // 실제 발사
                for (int j = 0; j < target.Count; j++)
                {
                    Vector3 start = transform.position;
                    Vector3 end = target[j].transform.position;

                    if (data.attackType == AttackType.POINT)
                        start = end;

                    PoolController.Pop(id, start, end);
                }

                float progressTime = data.projAttackTime / speedRatio;

                // 투사체가 공격을 입히는 시간 대기.
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

            // 디버프의 우선순위는 공격시마다 갱신되어야 함.
            // 전부 목록에서 제거, 추후 적이 살아있다면 추가하여 우선순위 재정렬.
            if (data.HasDebuff)
            {
                // 가장 앞의 하나를 지운 뒤 이어서 삭제
                enemies.Dequeue();
                for (int j = 1; j < target.Count; j++)
                    enemies.Remove(target[j]);
            }

            SoundController.PlayAudio(id);
            if (data.Stat(TowerStatType.SPLASH) != 0)
            {
                for (int j = 0; j < target.Count; j++)
                {
                    SplashPoint point = TowerController.Instance.PopSplashPoint();
                    point.transform.position = target[j].transform.position;
                    point.SetData(data, Stat(TowerStatType.DAMAGE));
                }
            }
            else EnemyController.Instance.EnemyAttacked(target, data, Stat(TowerStatType.DAMAGE));

            // 살아있는 적들을 추가함으로 우선순위 재정렬
            if (data.HasDebuff)
            {
                for (int j = 0; j < target.Count; j++)
                {
                    // 살아있는 경우에만 추가
                    if (target[j].gameObject.activeSelf)
                    {
                        enemies.Enqueue(target[j], GetPriority(target[j]));
                    }
                }
            }
        }
    }

    // 버프 부여 함수
    public void GiveBuff()
    {
        int targetAmount = (int)data.Stat(TowerStatType.MULTISHOT);
        if (targetAmount == 0) targetAmount = 1;
        List<TowerObject> target = towers.Get(targetAmount);
        if (target != null && towers.Count > 0)
        {
            Effect(target);
            for (int i = 0; i < target.Count; i++)
            {
                foreach (BuffType type in data.BuffTypes)
                {
                    if (type == BuffType.GOLDMINE) continue;
                    target[i].UpdateBuff(type, data.Buff(type), 5);
                }
            }

            // 버프의 우선순위는 전부 매 번 갱신해주어야함.
            // 전부 목록에서 제거 및 재추가.
            TowerObject[] tmp = new TowerObject[towers.Count];
            towers.CopyTo(tmp);
            towers.Clear();

            for (int i = 0; i < tmp.Length; i++)
            {
                towers.Enqueue(tmp[i], GetPriority(tmp[i]));
            }
        }
    }

    public void Effect(List<EnemyObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            // 타겟이 죽었을 경우에도 스플래시 공격이라면 이펙트가 남아야 함.
            // 반대로 타겟이 없고 스플래시 공격이 아니라면 이펙트가 남을 필요가 없음.
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

    // 버프 임시 이펙트
    public void Effect(List<TowerObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            GameObject effect = PoolController.PopEffect(data.id);

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
        // 공격일 경우 방향전환이 없으므로 미리 방향전환을 해둠.
        if (curAnim == AnimationType.ATTACK) LookAtEnemy();

        int number = 0;
        float time = 0;
        float frameTime = data.spf / speedRatio;
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
