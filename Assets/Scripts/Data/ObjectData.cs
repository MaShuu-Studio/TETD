using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ObjectData
{
    public int id { get; protected set; }
    public string name { get; protected set; }
}

public class Tower : ObjectData
{
    public int cost;

    public float range;
    public float dmg;

    public Tower(TowerData data)
    {
        id = data.id;
        name = "TOWER-" + data.name.ToUpper();

        cost = data.cost;
        range = data.range;
        dmg = data.dmg;
    }
}

public class Enemy : ObjectData
{
    public float hp;
    public float speed;
    public float dmg;

    public Enemy(EnemyData data)
    {
        id = data.id;
        name = "ENEMY-" + data.name.ToUpper();

        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;
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
    public int cost;

    public float range;
    public float dmg;
}

[Serializable]
public class EnemyData : JsonData
{
    public float hp;
    public float speed;
    public float dmg;
}
