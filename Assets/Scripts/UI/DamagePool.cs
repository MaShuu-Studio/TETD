using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePool : Pool
{
    public GameObject Pop(Vector3 pos, float damage)
    {
        if (pool.Count == 0) CreateObject();

        GameObject obj = pool.Pop();
        obj.name = pos.x + "," + pos.y + "," + damage;
        obj.SetActive(true);

        return obj;
    }
}
