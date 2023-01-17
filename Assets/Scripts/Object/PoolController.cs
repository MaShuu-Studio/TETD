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

    [SerializeField] private Pool poolPrefab;
    [SerializeField] private Poolable towerBase;
    [SerializeField] private Poolable enemyBase;
    
    private Dictionary <int, Pool> pool;

    public void Init()
    {
        pool = new Dictionary<int, Pool>();

        for (int i = 0; i < TowerManager.Keys.Count; i++)
        {
            int id = TowerManager.Keys[i];
            GameObject go = Instantiate(towerBase.gameObject);
            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable.MakePrefab(id) == false) continue;

            GameObject poolObject = Instantiate(poolPrefab.gameObject, transform);
            poolObject.name = id.ToString();
            Pool poolComponent = poolObject.GetComponent<Pool>();
            poolComponent.Init(poolable);

            pool.Add(id, poolComponent);
        }

        for (int i = 0; i < EnemyManager.Keys.Count; i++)
        {
            int id = EnemyManager.Keys[i];
            GameObject go = Instantiate(enemyBase.gameObject);
            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable.MakePrefab(id) == false) continue;

            GameObject poolObject = Instantiate(poolPrefab.gameObject, transform);
            poolObject.name = id.ToString();
            Pool poolComponent = poolObject.GetComponent<Pool>();
            poolComponent.Init(poolable);

            pool.Add(id, poolComponent);
        }

        /* Poolable의 존재에 따라 자동으로 Object Pool에 넣어주는 시스템
        List<Poolable> list = ResourceManager.GetResources<Poolable>("Prefab");
        for (int i = 0; i < list.Count; i++)
        {
            string objName = list[i].gameObject.name;
            GameObject poolObject = Instantiate(poolPrefab.gameObject, transform);
            poolObject.name = objName;
            Pool poolComponent = poolObject.GetComponent<Pool>();
            poolComponent.Init(list[i]);

            pool.Add(objName.ToUpper(), poolComponent);
        }
        */
    }

    public static void Push(int id, GameObject obj)
    {
        if (Instance.pool.ContainsKey(id) == false) return;

        Instance.pool[id].Push(obj);
    }

    public static GameObject Pop(int id)
    {
        if (Instance.pool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.pool[id].Pop();
        return obj;
    }
}
