using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    protected Poolable objPrefab;

    protected Stack<GameObject> pool;

    public void Init(Poolable prefab)
    {
        objPrefab = prefab;
        if (objPrefab.gameObject.activeSelf)
        {
            objPrefab.gameObject.name = "BASE";
            objPrefab.transform.SetParent(transform);
            objPrefab.gameObject.SetActive(false);
        }
        pool = new Stack<GameObject>();

        for (int i = 0; i < prefab.Amount; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        Poolable poolable = Instantiate(objPrefab);
        poolable.MakePrefab(objPrefab.Id);
        Push(poolable.gameObject);
    }

    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Push(obj);
    }

    public GameObject Pop()
    {
        if (pool.Count == 0) CreateObject();

        GameObject obj = pool.Pop();
        obj.SetActive(true);

        return obj;
    }
}
