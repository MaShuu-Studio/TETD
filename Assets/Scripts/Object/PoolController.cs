using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class PoolController : MonoBehaviour
{
    #region Instance
    public static PoolController Instance { get { return instance; } }
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
    [Header("Parent")]
    [SerializeField] private Transform towerParent;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private Transform projParent;
    [SerializeField] private Transform effectParent;

    [Header("Object")]
    [SerializeField] private Pool poolPrefab;
    [SerializeField] private Poolable towerBase;
    [SerializeField] private Poolable enemyBase;

    [Space]
    [SerializeField] private ProjectilePool projPoolPrefab;
    [SerializeField] private Poolable projBase;

    [Space]
    [SerializeField] private Poolable effectBase;

    private Dictionary<int, Pool> pool;
    private Dictionary<int, ProjectilePool> projPool;
    private Dictionary<int, Pool> effectPool;

    public void Init()
    {
        pool = new Dictionary<int, Pool>();
        projPool = new Dictionary<int, ProjectilePool>();
        effectPool = new Dictionary<int, Pool>();

        for (int i = 0; i < TowerManager.Keys.Count; i++)
        {
            int id = TowerManager.Keys[i];
            Poolable poolable = Instantiate(towerBase);
            if (poolable.MakePrefab(id) == false)
            {
                Destroy(poolable.gameObject);
                continue;
            }

            Pool poolComponent = Instantiate(poolPrefab, towerParent);
            poolComponent.gameObject.name = id.ToString();
            poolComponent.Init(poolable);

            pool.Add(id, poolComponent);

            // 투사체 추가부분
            // 투사체가 존재하는 유닛일 경우에만 추가
            if (TowerManager.Projs.ContainsKey(id))
            {
                Poolable proj = Instantiate(projBase);
                if (proj.MakePrefab(id) == false)
                {
                    Destroy(proj.gameObject);
                }
                else
                {
                    ProjectilePool projPoolComponent = Instantiate(projPoolPrefab, projParent);
                    projPoolComponent.gameObject.name = id.ToString();
                    projPoolComponent.Init(proj);

                    projPool.Add(id, projPoolComponent);
                }
            }

            // 이펙트 추가부분
            // 이펙트가 존재하는 유닛일 경우에만 추가
            if (TowerManager.Effects.ContainsKey(id))
            {
                Poolable effect = Instantiate(effectBase);
                if (effect.MakePrefab(id) == false)
                {
                    Destroy(effect.gameObject);
                }
                else
                {
                    Pool effectPoolComponent = Instantiate(poolPrefab, effectParent);
                    effectPoolComponent.gameObject.name = id.ToString();
                    effectPoolComponent.Init(effect);

                    effectPool.Add(id, effectPoolComponent);
                }
            }
        }

        for (int i = 0; i < EnemyManager.Keys.Count; i++)
        {
            int id = EnemyManager.Keys[i];
            Poolable poolable = Instantiate(enemyBase);
            if (poolable.MakePrefab(id) == false)
            {
                Destroy(poolable.gameObject);
                continue;
            }

            Pool poolComponent = Instantiate(poolPrefab, enemyParent);
            poolComponent.gameObject.name = id.ToString();
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

    // 투사체 전용 Push
    public static void PushProj(int id, GameObject obj)
    {
        if (Instance.projPool.ContainsKey(id) == false) return;

        Instance.projPool[id].Push(obj);
    }
    
    // 투사체 전용 Pop
    public static GameObject Pop(int id, Vector2 start, Vector2 end)
    {
        if (Instance.projPool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.projPool[id].Pop(start, end);
        return obj;
    }

    // 이펙트 전용 Push
    public static void PushEffect(int id, GameObject obj)
    {
        if (Instance.effectPool.ContainsKey(id) == false) return;

        Instance.effectPool[id].Push(obj);
    }

    // 이펙트 전용 Pop
    public static GameObject PopEffect(int id)
    {
        if (Instance.effectPool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.effectPool[id].Pop();
        return obj;
    }
}
