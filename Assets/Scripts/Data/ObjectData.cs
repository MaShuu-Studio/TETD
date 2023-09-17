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
    public Vector2 pivot;

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

    public bool HasDebuff { get; private set; }
    public bool HasBuff { get; private set; }

    public List<AbilityType> AbilityTypes { get; private set; }

    private Dictionary<TowerStatType, float> stat;
    private Dictionary<TowerStatType, int> statLevel;
    private Dictionary<AbilityType, TowerAbility> abilities;

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
    public static Color Color(Grade grade)
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

    public Tower(TowerData data, Dictionary<AnimationType, Sprite[]> animation)
    {
        id = data.id;
        pivot = data.pivot;

        this.animation = animation;

        /* id: ABBCDDD
         * A: type
         * B: element
         * C: grade
         * D: num 
         */

        string checker = id.ToString().Substring(1, 3); // BBC
        string e = checker.Substring(0, 2);
        element = (Element)(int.Parse(e));

        string g = checker.Substring(2, 1);
        grade = (Grade)(int.Parse(g));

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

        abilities = new Dictionary<AbilityType, TowerAbility>();
        AbilityTypes = new List<AbilityType>();
        if (data.ability != null)
        {
            for (int i = 0; i < data.ability.Count; i++)
            {
                abilities.Add((AbilityType)data.ability[i].type, data.ability[i]);
                AbilityTypes.Add((AbilityType)data.ability[i].type);

                if (IsBuff(data.ability[i].type)) HasBuff = true;
                else if (IsDebuff(data.ability[i].type)) HasDebuff = true;
            }
        }
    }

    public Tower(Tower data)
    {
        id = data.id;
        name = data.name;
        animation = data.animation;

        pivot = data.pivot;

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
        foreach (TowerStatType type in data.stat.Keys)
        {
            statLevel.Add(type, 1);
        }

        abilities = new Dictionary<AbilityType, TowerAbility>();
        AbilityTypes = new List<AbilityType>();
        if (data.abilities.Count > 0)
        {
            foreach (AbilityType type in data.abilities.Keys)
            {
                abilities.Add(type, new TowerAbility(data.abilities[type]));
                AbilityTypes.Add(type);

                if (IsBuff(type)) HasBuff = true;
                else if (IsDebuff(type)) HasDebuff = true;
            }
        }
    }

    public static bool IsBuff(int type)
    {
        return (type >= 100 && type < 200);
    }

    public static bool IsBuff(AbilityType type)
    {
        return ((int)type >= 100 && (int)type < 200);
    }

    public static bool IsDebuff(int type)
    {
        return (type >= 200 && type < 300);
    }

    public static bool IsDebuff(AbilityType type)
    {
        return ((int)type >= 200 && (int)type < 300);
    }

    public float Stat(TowerStatType type)
    {
        if (stat.ContainsKey(type)) return stat[type];
        return 0;
    }

    public TowerAbility Ability(AbilityType type)
    {
        if (abilities.ContainsKey(type)) return abilities[type];
        return null;
    }

    public int StatLevel(int id)
    {
        int check = id / 1000;
        if (check == ((int)SpriteManager.ETCDataNumber.TOWERSTAT / 1000))
        {
            TowerStatType type = (TowerStatType)(id % 1000);
            if (statLevel.ContainsKey(type))
            {
                return statLevel[type];
            }
        }
        else if (check == ((int)SpriteManager.ETCDataNumber.TOWERABILITY / 1000))
        {
            AbilityType type = (AbilityType)(id % 1000);
            if (abilities.ContainsKey(type))
                return abilities[type].lv;
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
                upgradeCost += cost / 10;
                value += upgradeCost;
            }
        }

        foreach (var ability in abilities.Values)
        {
            int level = ability.lv;

            int upgradeCost = 0;

            for (int i = 1; i < level; i++)
            {
                isUpgrade = true;
                if (ability.type == (int)AbilityType.MULTISHOT)
                    upgradeCost += cost / 2;
                else
                    upgradeCost += cost / 10;
                value += upgradeCost;
            }
        }

        if (isUpgrade) value = Mathf.CeilToInt(value * 0.7f);

        return value;
    }

    public void Upgrade(int id)
    {
        int check = id / 1000;
        if (check == ((int)SpriteManager.ETCDataNumber.TOWERSTAT / 1000))
        {
            TowerStatType type = (TowerStatType)(id % 1000);

            statLevel[type]++;
            stat[type] *= 1.1f;
        }
        else if (check == ((int)SpriteManager.ETCDataNumber.TOWERABILITY / 1000))
        {
            AbilityType type = (AbilityType)(id % 1000);

            abilities[type].lv++;
            if (type == AbilityType.MULTISHOT)
                abilities[type].value += 1;
            else
            {
                abilities[type].value *= 1.1f;
            }
        }
    }

    public int UpgradeCost(int id)
    {
        int upgradeCost = 0;

        int check = id / 1000;
        if (check == ((int)SpriteManager.ETCDataNumber.TOWERSTAT / 1000))
        {
            TowerStatType type = (TowerStatType)(id % 1000);

            int level = statLevel[type];
            for (int i = 1; i <= level; i++)
            {
                upgradeCost += cost / 10;
            }
        }
        else if (check == ((int)SpriteManager.ETCDataNumber.TOWERABILITY / 1000))
        {
            AbilityType type = (AbilityType)(id % 1000);

            int level = abilities[type].lv;
            for (int i = 1; i <= level; i++)
            {
                if (type == AbilityType.MULTISHOT)
                    upgradeCost += cost / 2;
                else
                    upgradeCost += cost / 10;
            }
        }
        return upgradeCost;
    }
}

public class Enemy : ObjectData
{
    public Element element;
    public EnemyGrade grade;

    public Vector2 pivot;

    public int money;
    public int exp;

    public float hp;
    public float speed;
    public float dmg;

    public float spf;
    public Sprite Mask { get; private set; }
    public float Height { get; private set; }
    public Color GradeColor
    {
        get
        {
            switch (grade)
            {
                case EnemyGrade.NORMAL:
                    return new Color(.8f, .8f, .8f);
                case EnemyGrade.ELITE:
                    return new Color(0.7f, 0.4f, 1);
                default:
                    return new Color(1, 0.2f, 0.2f);
            }
        }
    }
    public static Color Color(EnemyGrade grade)
    {
        switch (grade)
        {
            case EnemyGrade.NORMAL:
                return new Color(.8f, .8f, .8f);
            case EnemyGrade.ELITE:
                return new Color(0.7f, 0.4f, 1);
            default:
                return new Color(1, 0.2f, 0.2f);
        }
    }

    public Enemy(EnemyData data, Dictionary<AnimationType, Sprite[]> animation)
    {
        id = data.id;
        this.animation = animation;

        pivot = data.pivot;

        // 높이는 Mask Height / Pixel Per Unit 임.
        // Mask Height = 스프라이트 전체 사이즈의 높이가 담겨있음.
        // Pixel Per Unit = Scene에서의 1의 사이즈가 몇 픽셀인지 담겨 있음.
        Height = animation[AnimationType.MOVE][0].texture.height / data.pixelperunit;

        /* id: ABBCDDD
         * A: type
         * B: element
         * C: grade
         * D: num 
         */

        string checker = id.ToString().Substring(1, 3); // BBC
        string e = checker.Substring(0, 2);
        element = (Element)(int.Parse(e));

        string g = checker.Substring(2, 1);
        grade = (EnemyGrade)(int.Parse(g));

        exp = data.exp;
        money = data.money;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;

        if (data.spf == 0) data.spf = 0.1f;
        spf = data.spf;
    }

    public Enemy(Enemy data)
    {
        id = data.id;
        animation = data.animation;
        Height = data.Height;

        pivot = data.pivot;

        element = data.element;
        grade = data.grade;

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

[Serializable]
public class JsonData
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
    public int lv = 1;
    public float time; // 지속시간
    public float atkSpeed; // 공속
    public float value;

    public TowerAbility()
    {

    }
    public TowerAbility(TowerAbility ability)
    {
        type = ability.type;
        lv = ability.lv;
        time = ability.time;
        atkSpeed = ability.atkSpeed;
        value = ability.value;
    }
}
