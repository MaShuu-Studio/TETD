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

[Serializable]
public class TowerData
{
    public int id;
    public string name;

    public int cost;

    public float range;
    public float dmg;
}

[Serializable]
public class EnemyData
{
    public int id;
    public string name;

    public float hp;
    public float speed;
    public float dmg;
}
