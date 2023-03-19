using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : Pool
{
    public GameObject Pop(Vector2 start, Vector2 end)
    {
        if (pool.Count == 0) CreateObject();

        GameObject obj = pool.Pop();
        obj.name = start.x + "," + start.y + ";" + end.x + "," + end.y;
        obj.SetActive(true);

        return obj;
    }
}
