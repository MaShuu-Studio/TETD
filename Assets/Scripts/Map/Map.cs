using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Map
{
    public Vector2 mapSize;
    public List<Vector2> enemyRoad;

    public bool Buildable(Vector2 pos)
    {
        for(int i= 0; i < enemyRoad.Count; i++)
        {
            if (pos.x == enemyRoad[i].x && pos.y == enemyRoad[i].y) 
                return true;
        }
        return false;
    }
}
