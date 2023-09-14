using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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
    [SerializeField] private Transform buffParent;

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
    private Dictionary<int, Pool> hitEffectPool;
    private Dictionary<int, Pool> buffEffectPool;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static void GetTotal()
    {
        TotalProgress = TowerManager.TotalProgress + EnemyManager.TotalProgress;
    }
    public async Task Init()
    {
        pool = new Dictionary<int, Pool>();
        projPool = new Dictionary<int, ProjectilePool>();
        hitEffectPool = new Dictionary<int, Pool>();
        buffEffectPool = new Dictionary<int, Pool>();

        while (TowerManager.isLoaded == false || EnemyManager.isLoaded == false) await Task.Yield();

        for (int i = 0; i < TowerManager.Keys.Count; i++)
        {
            CurProgress++;

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
            if (TowerManager.HitEffects.ContainsKey(id))
            {
                Poolable effect = Instantiate(effectBase);
                if (effect.MakePrefab(id + EffectObject.CheckBuffEffect * 1) == false)
                {
                    Destroy(effect.gameObject);
                }
                else
                {
                    Pool effectPoolComponent = Instantiate(poolPrefab, effectParent);
                    effectPoolComponent.gameObject.name = id.ToString();
                    effectPoolComponent.Init(effect);

                    hitEffectPool.Add(id, effectPoolComponent);
                }
            }

            // 버프 이펙트 추가부분
            if (TowerManager.BuffEffects.ContainsKey(id))
            {
                Poolable effect = Instantiate(effectBase);
                if (effect.MakePrefab(id + EffectObject.CheckBuffEffect * 2) == false)
                {
                    Destroy(effect.gameObject);
                }
                else
                {
                    Pool effectPoolComponent = Instantiate(poolPrefab, buffParent);
                    effectPoolComponent.gameObject.name = id.ToString();
                    effectPoolComponent.Init(effect);

                    buffEffectPool.Add(id, effectPoolComponent);
                }
            }
        }

        for (int i = 0; i < EnemyManager.Keys.Count; i++)
        {
            CurProgress++;

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
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD POOLABLE OBJECT {CurProgress}");
#endif
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
        if (Instance.hitEffectPool.ContainsKey(id) == false) return;

        Instance.hitEffectPool[id].Push(obj);
    }

    // 이펙트 전용 Pop
    public static GameObject PopEffect(int id)
    {
        if (Instance.hitEffectPool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.hitEffectPool[id].Pop();
        return obj;
    }

    // 버프 이펙트 전용 Push
    public static void PushBuffEffect(int id, GameObject obj)
    {
        if (Instance.buffEffectPool.ContainsKey(id) == false) return;

        Instance.buffEffectPool[id].Push(obj);
    }

    // 버프 이펙트 전용 Pop
    public static GameObject PopBuffEffect(int id)
    {
        if (Instance.buffEffectPool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.buffEffectPool[id].Pop();
        return obj;
    }
}
