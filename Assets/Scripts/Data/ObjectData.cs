using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using EnumData;

public abstract class ObjectData
{
    public int id { get; protected set; }
    public string name { get; protected set; }
    public Dictionary<AnimationType, Sprite[]> animation { get; protected set; }

    public void UpdateName(Language lang)
    {
        name = lang.name;
    }
}

public class Tower : ObjectData
{
    public Grade grade;
    public Element element;

    public AttackType attackType;

    public int cost;

    public float spf;
    public float[] attackTime;
    public int AttackAmount { get { return attackTime.Length; } }

    public float projspf;
    public float projAttackTime;
    public float projTime;

    public float effectspf;
    public Color effectColor;

    public bool HasDebuff { get { return debuffs != null && debuffs.Count > 0; } }
    public bool HasBuff { get { return buffs != null && buffs.Count > 0; } }

    public TowerStatType[] StatTypes { get; private set; }
    public BuffType[] BuffTypes { get; private set; }
    public DebuffType[] DebuffTypes { get; private set; }

    private Dictionary<TowerStatType, float> stat;
    private Dictionary<TowerStatType, int> statLevel;
    private Dictionary<BuffType, float> buffs;
    private Dictionary<DebuffType, float> debuffs;
    public Color GradeColor
    {
        get
        {
            switch (grade)
            {
                case Grade.NORMAL:
                    return new Color(.8f, .8f, .8f);
                case Grade.RARE:
                    return new Color(0.5f, 0.8f, 1);
                case Grade.HEROIC:
                    return new Color(0.7f, 0.4f, 1);
                default:
                    return new Color(1, 0.4f, 0.2f);
            }
        }
    }

    public Tower(TowerData data, Dictionary<AnimationType, Sprite[]> animation)
    {
        id = data.id;

        this.animation = animation;

        /* id: ABBCDDD
         * A: type
         * B: element
         * C: grade
         * D: num 
         */

        int tmp = id / 1000; // ABBC
        tmp %= 1000; // BBC
        element = (Element)(tmp / 10); // BB
        grade = (Grade)(tmp % 10); // C

        attackType = data.type;
        projspf = data.projspf;
        projAttackTime = data.projattacktime;
        projTime = data.projtime;

        effectspf = data.effectspf;
        effectColor = data.effectcolor;

        cost = data.cost;

        spf = data.spf;
        attackTime = new float[data.attacktime.Count];
        data.attacktime.CopyTo(attackTime);

        // 스탯
        stat = new Dictionary<TowerStatType, float>();
        stat.Add(TowerStatType.DAMAGE, data.dmg);
        // 데이터에는 공격 딜레이를 저장해두기 때문에 로드 시에만 역수로
        // 해당 로드 이후에는 공격속도로 변환됨.
        stat.Add(TowerStatType.ATTACKSPEED, 1 / data.attackspeed);
        stat.Add(TowerStatType.DISTANCE, data.range);
        if (data.ability != null)
        {
            for (int i = 0; i < data.ability.Count; i++)
            {
                stat.Add((TowerStatType)data.ability[i].type, data.ability[i].value);
            }
        }
        StatTypes = stat.Keys.ToArray();

        // 버프, 디버프
        buffs = new Dictionary<BuffType, float>();
        if (data.buffs != null && data.buffs.Count > 0)
        {
            for (int i = 0; i < data.buffs.Count; i++)
            {
                buffs.Add((BuffType)data.buffs[i].type, data.buffs[i].value);
            }
            BuffTypes = buffs.Keys.ToArray();
        }

        debuffs = new Dictionary<DebuffType, float>();
        if (data.debuffs != null && data.debuffs.Count > 0)
        {
            for (int i = 0; i < data.debuffs.Count; i++)
            {
                debuffs.Add((DebuffType)data.debuffs[i].type, data.debuffs[i].value);
            }
            DebuffTypes = debuffs.Keys.ToArray();
        }

    }

    public Tower(Tower data)
    {
        id = data.id;
        name = data.name;
        animation = data.animation;

        attackType = data.attackType;
        projspf = data.projspf;
        projAttackTime = data.projAttackTime;
        projTime = data.projTime;

        effectspf = data.effectspf;
        effectColor = data.effectColor;

        grade = data.grade;
        element = data.element;

        cost = data.cost;

        spf = data.spf;
        attackTime = new float[data.attackTime.Length];
        data.attackTime.CopyTo(attackTime, 0);

        // 스탯
        stat = new Dictionary<TowerStatType, float>(data.stat);
        statLevel = new Dictionary<TowerStatType, int>();
        foreach (TowerStatType stat in data.stat.Keys)
        {
            statLevel.Add(stat, 1);
        }
        StatTypes = stat.Keys.ToArray();

        // 버프, 디버프
        buffs = new Dictionary<BuffType, float>();
        if (data.buffs.Count > 0)
        {
            foreach (BuffType type in data.buffs.Keys)
            {
                buffs.Add(type, data.buffs[type]);
            }
            BuffTypes = buffs.Keys.ToArray();
        }

        debuffs = new Dictionary<DebuffType, float>();
        if (data.debuffs.Count > 0)
        {
            foreach (DebuffType type in data.debuffs.Keys)
            {
                debuffs.Add(type, data.debuffs[type]);
            }
            DebuffTypes = debuffs.Keys.ToArray();
        }
    }

    public float Stat(TowerStatType type)
    {
        if (stat.ContainsKey(type)) return stat[type];
        return 0;
    }

    public float Buff(BuffType type)
    {
        if (buffs.ContainsKey(type)) return buffs[type];
        return 0;
    }

    public float Debuff(DebuffType type)
    {
        if (debuffs.ContainsKey(type)) return debuffs[type];
        return 0;
    }

    public int StatLevel(TowerStatType type)
    {
        if (statLevel.ContainsKey(type))
        {
            return statLevel[type];
        }

        return 0;
    }

    public int Value()
    {
        int value = cost;
        bool isUpgrade = false;
        foreach (var kv in statLevel)
        {
            TowerStatType type = kv.Key;
            int level = kv.Value;

            int upgradeCost = 0;

            for (int i = 1; i < level; i++)
            {
                isUpgrade = true;
                if (type == TowerStatType.MULTISHOT)
                    upgradeCost += cost;
                else
                    upgradeCost += cost / 10;
                value += upgradeCost;
            }
        }

        if (isUpgrade) value = Mathf.CeilToInt(value * 0.7f);

        return value;
    }

    public void Upgrade(TowerStatType type)
    {
        statLevel[type]++;
        if (type == TowerStatType.MULTISHOT)
            stat[type] += 1;
        else
            stat[type] *= 1.1f;
    }

    public int UpgradeCost(TowerStatType type)
    {
        int upgradeCost = 0;
        int level = statLevel[type];
        for (int i = 1; i <= level; i++)
        {
            if (type == TowerStatType.MULTISHOT)
                upgradeCost += cost / 2;
            else
                upgradeCost += cost / 10;
        }

        return upgradeCost;
    }
}

public class Enemy : ObjectData
{
    public Element element;

    public int money;
    public int exp;

    public float hp;
    public float speed;
    public float dmg;

    public float spf;
    public Sprite Mask { get; private set; }
    public float Height { get; private set; }

    public Enemy(EnemyData data, Dictionary<AnimationType, Sprite[]> animation, Sprite mask)
    {
        id = data.id;
        this.animation = animation;
        Mask = mask;

        // 높이는 Mask Height / Pixel Per Unit 임.
        // Mask Height = 스프라이트 전체 사이즈의 높이가 담겨있음.
        // Pixel Per Unit = Scene에서의 1의 사이즈가 몇 픽셀인지 담겨 있음.
        Height = Mask.texture.height / data.pixelperunit;

        /* id: ABBCDDD
         * A: type
         * B: element
         * C: grade
         * D: num 
         */

        int tmp = id / 1000; // ABBC
        tmp %= 1000; // BBC
        element = (Element)(tmp / 10); // BB
        //grade = (Grade)(tmp % 10); // C

        exp = data.exp;
        money = data.money;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;

        spf = data.spf;
    }

    public Enemy(Enemy data)
    {
        id = data.id;
        animation = data.animation;
        Mask = data.Mask;
        Height = data.Height;

        element = data.element;

        exp = data.exp;
        money = data.money;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;

        spf = data.spf;
    }

    public Element WeakElement()
    {
        Element weak = element + 1;
        if (weak > EnumArray.Elements[EnumArray.Elements.Length - 1]) weak = EnumArray.Elements[0];

        return weak;
    }

    public Element StrongElement()
    {
        Element strong = element - 1;
        if (strong < EnumArray.Elements[0]) strong = EnumArray.Elements[EnumArray.Elements.Length - 1];

        return strong;
    }
}

public abstract class JsonData
{
    public int id;
    public string name;

    public string imgsrc;
    public Vector2 pivot;
    public float pixelperunit = 24;
}

[Serializable]
public class TowerData : JsonData
{
    public Grade grade;
    public Element element;

    public AttackType type;

    public List<TowerAbility> ability;
    public List<TowerAbility> buffs;
    public List<TowerAbility> debuffs;

    public int cost;

    public float dmg;
    public float attackspeed;
    public float range;

    public float spf;
    public List<float> attacktime;

    public float projspf;
    public float projattacktime;
    public float projtime;

    public float effectspf = 0.03f;
    public Color effectcolor = Color.white;
}

[Serializable]
public class EnemyData : JsonData
{
    public Element element;

    public int money;
    public int exp;

    public float hp;
    public float speed;
    public float dmg;

    public float spf;
}

[Serializable]
public class TowerAbility
{
    public int type; // enumType을 전부 받을 수 있도록
    public float value;
}
