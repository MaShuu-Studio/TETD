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
        objPrefab.transform.parent = transform;
        objPrefab.gameObject.name = "BASE";
        objPrefab.gameObject.SetActive(false);

        pool = new Stack<GameObject>();

        for (int i = 0; i < prefab.Amount; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        GameObject obj = Instantiate(objPrefab.gameObject);
        Poolable poolable = obj.GetComponent<Poolable>();
        poolable.MakePrefab(objPrefab.Id);
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
