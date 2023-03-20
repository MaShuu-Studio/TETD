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

    public void UpdateName(Language lang, LanguageType type)
    {
        name = lang.GetName(type);
    }
}

public class Tower : ObjectData
{
    public Grade grade;
    public Element element;

    public int cost;

    public float spf;
    public float attackTime;

    public bool hasDebuff;

    public List<TowerStatType> StatTypes { get { return stat.Keys.ToList(); } }
    private Dictionary<TowerStatType, float> stat;
    private Dictionary<TowerStatType, int> statLevel;

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

        cost = data.cost;

        spf = data.spf;
        attackTime = data.attacktime;

        hasDebuff = false;

        stat = new Dictionary<TowerStatType, float>();
        stat.Add(TowerStatType.DAMAGE, data.dmg);
        stat.Add(TowerStatType.ATTACKSPEED, data.attackspeed);
        stat.Add(TowerStatType.DISTANCE, data.range);
        if (data.ability != null)
        {
            for (int i = 0; i < data.ability.Count; i++)
            {
                if (data.ability[i].type >= TowerStatType.DOTDAMAGE) hasDebuff = true;

                stat.Add(data.ability[i].type, data.ability[i].value);
            }
        }
    }

    public Tower(Tower data)
    {
        id = data.id;
        animation = data.animation;

        grade = data.grade;
        element = data.element;

        cost = data.cost;

        spf = data.spf;
        attackTime = data.attackTime;

        hasDebuff = data.hasDebuff;

        stat = new Dictionary<TowerStatType, float>(data.stat);
        statLevel = new Dictionary<TowerStatType, int>();
        foreach (TowerStatType stat in data.stat.Keys)
        {
            statLevel.Add(stat, 1);
        }
    }

    public float Stat(TowerStatType type)
    {
        if (stat.ContainsKey(type))
        {
            return stat[type];
        }

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

    public Enemy(EnemyData data, Dictionary<AnimationType, Sprite[]> animation)
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
    public float pixelperunit;
}

[Serializable]
public class TowerData : JsonData
{
    public Grade grade;
    public Element element;

    public List<TowerAbility> ability;

    public int cost;

    public float dmg;
    public float attackspeed;
    public float range;

    public float spf;
    public float attacktime;
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
    public TowerStatType type;
    public float value;
}
