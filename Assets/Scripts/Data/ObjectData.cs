using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EnumData;

public abstract class ObjectData
{
    public int id { get; protected set; }
    public string name { get; protected set; }
}

public class Tower : ObjectData
{
    public Grade grade;
    public Element element;

    public int cost;

    public Dictionary<TowerMainStatType, float> stat;
    public Dictionary<TowerMainStatType, int> statLevel;

    public Tower(TowerData data)
    {
        id = data.id;
        name = "TOWER-" + data.name.ToUpper();

        grade = data.grade;
        element = data.element;

        cost = data.cost;

        stat = new Dictionary<TowerMainStatType, float>();
        stat.Add(TowerMainStatType.DAMAGE, data.dmg);
        stat.Add(TowerMainStatType.ATTACKSPEED, data.attackspeed);
        stat.Add(TowerMainStatType.DISTANCE, data.range);
    }

    public Tower(Tower data)
    {
        id = data.id;
        name = data.name;

        grade = data.grade;
        element = data.element;

        cost = data.cost;

        stat = new Dictionary<TowerMainStatType, float>(data.stat);

        statLevel = new Dictionary<TowerMainStatType, int>();
        for (int i = 0; i < EnumArray.TowerMainStatTypes.Length; i++)
        {
            statLevel.Add((TowerMainStatType)i, 1);
        }
    }

    public int Value()
    {
        int value = cost;
        bool isUpgrade = false;
        foreach(var level in statLevel.Values)
        {
            int upgradeCost = 0;

            for (int i = 1; i < level; i++)
            {
                isUpgrade = true;
                upgradeCost += cost / 10;
                value += upgradeCost;
            }
        }

        if (isUpgrade) value = Mathf.CeilToInt(value * 0.7f);

        return value;
    }

    public void Upgrade(TowerMainStatType type)
    {
        statLevel[type]++;
        stat[type] *= 1.1f;
    }

    public int UpgradeCost(TowerMainStatType type)
    {
        int upgradeCost = 0;
        int level = statLevel[type];

        for (int i = 1; i <= level; i++)
        {
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

    public Enemy(EnemyData data)
    {
        id = data.id;
        name = "ENEMY-" + data.name.ToUpper();

        element = data.element;

        exp = data.exp;
        money = data.money;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;
    }
    public Enemy(Enemy data)
    {
        id = data.id;
        name = data.name;

        element = data.element;

        exp = data.exp;
        money = data.money;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;
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

    public int cost;

    public float dmg;
    public float attackspeed;
    public float range;
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
}
