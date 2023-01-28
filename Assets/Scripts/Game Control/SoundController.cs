using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get { return instance; } }
    private static SoundController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    [SerializeField] private Pool poolPrefab;
    [SerializeField] private Poolable soundBase;
    private Dictionary<int, Pool> pool;

    public float SfxVolume { get { return sfxVolume; } }
    private float bgmVolume;
    private float sfxVolume;

    public void Init()
    {
        bgmVolume = 1;
        sfxVolume = 1;

        pool = new Dictionary<int, Pool>();

        for (int i = 0; i < SoundManager.Keys.Count; i++)
        {
            int id = 0;
            if (int.TryParse(SoundManager.Keys[i], out id) == false) continue;
            GameObject go = Instantiate(soundBase.gameObject);
            Poolable poolable = go.GetComponent<Poolable>();
            if (poolable.MakePrefab(id) == false) continue;

            GameObject poolObject = Instantiate(poolPrefab.gameObject, transform);
            poolObject.name = id.ToString();
            Pool poolComponent = poolObject.GetComponent<Pool>();
            poolComponent.Init(poolable);

            pool.Add(id, poolComponent);
        }
    }

    public static void StopAudio(int id, GameObject obj)
    {
        if (Instance.pool.ContainsKey(id) == false) return;

        Instance.pool[id].Push(obj);
    }

    public static GameObject PlayAudio(int id)
    {
        if (Instance.pool.ContainsKey(id) == false) return null;

        GameObject obj = Instance.pool[id].Pop();
        return obj;
    }

    public void SetVolume(float bgm, float sfx)
    {
        bgmVolume = bgm / 10;
        sfxVolume = sfx / 10;
    }
}
