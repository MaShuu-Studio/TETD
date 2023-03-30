using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class ProjectilePool : Pool
{
    public GameObject Pop(bool loop, float spf, float speed, Vector2 start, Vector2 end)
    {
        if (pool.Count == 0) CreateObject();

        GameObject obj = pool.Pop();
        if (loop) obj.name = "T;";
        else obj.name = "F;";
        obj.name += $"{spf};{speed};{start.x},{start.y};{end.x},{end.y}";
        obj.SetActive(true);

        return obj;
    }
}
