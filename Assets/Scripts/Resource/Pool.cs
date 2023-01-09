using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private Poolable objPrefab;

    Stack<GameObject> pool;

    public void Init(Poolable prefab)
    {
        objPrefab = prefab;
        pool = new Stack<GameObject>();

        for (int i = 0; i < prefab.Amount; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        GameObject obj = Instantiate(objPrefab.gameObject);
        obj.name = objPrefab.gameObject.name;
        Push(obj);
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
