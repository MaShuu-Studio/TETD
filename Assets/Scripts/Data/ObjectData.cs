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

    public float range;
    public float dmg;
    public float attackspeed;

    public Tower(TowerData data)
    {
        id = data.id;
        name = "TOWER-" + data.name.ToUpper();

        grade = data.grade;
        element = data.element;

        cost = data.cost;
        range = data.range;
        dmg = data.dmg;
        attackspeed = data.attackspeed;
    }

    public Tower(Tower data)
    {
        id = data.id;
        name = data.name;

        grade = data.grade;
        element = data.element;

        cost = data.cost;
        range = data.range;
        dmg = data.dmg;
        attackspeed = data.attackspeed;
    }
}

public class Enemy : ObjectData
{
    public Element element;

    public float hp;
    public float speed;
    public float dmg;

    public Enemy(EnemyData data)
    {
        id = data.id;
        name = "ENEMY-" + data.name.ToUpper();

        element = data.element;

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;
    }
    public Enemy(Enemy data)
    {
        id = data.id;
        name = data.name;

        element = data.element;

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

    public float range;
    public float dmg;
    public float attackspeed;
}

[Serializable]
public class EnemyData : JsonData
{
    public Element element;

    public float hp;
    public float speed;
    public float dmg;
}
