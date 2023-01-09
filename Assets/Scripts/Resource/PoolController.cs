using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    #region Instance
    public static  PoolController Instance { get { return instance; } }
    private static PoolController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    Dictionary<string, Pool> pool;

    public void Init()
    {
        pool = new Dictionary<string, Pool>();
        List<Poolable> list = ResourceManager.GetResources<Poolable>("");
        for (int i = 0; i < list.Count; i++)
        {
            string objName = list[i].gameObject.name;
            GameObject poolObject = new GameObject(objName);
            poolObject.transform.SetParent(transform);
            Pool poolComponent = poolObject.AddComponent<Pool>();
            poolComponent.Init(list[i]);

            pool.Add(objName.ToUpper(), poolComponent);
        }
    }

    public void Push(string name, GameObject obj)
    {
        name = name.ToUpper();

        if (pool.ContainsKey(name) == false) return;

        pool[name].Push(obj);
    }

    public GameObject Pop(string name)
    {
        name = name.ToUpper();

        if (pool.ContainsKey(name) == false) return null;

        GameObject obj = pool[name].Pop();
        return obj;
    }

    // Å×½ºÆ®
    Stack<GameObject> stack = new Stack<GameObject>();
    private void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(0, 0), new Vector2(100, 50)), "PUSH"))
        {
            if (stack.Count > 0)
            {
                GameObject obj = stack.Pop();

                Push("Poolable", obj);
            }
        }

        if (GUI.Button(new Rect(new Vector2(0, 100), new Vector2(100, 50)), "POP"))
        {
            GameObject obj = Pop("Poolable");

            obj.transform.position = new Vector3(
                Random.Range(-5, 5), Random.Range(-5, 5), 0);
            stack.Push(obj);
        }
    }
}
