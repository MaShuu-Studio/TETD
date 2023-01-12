using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // 후에 몬스터 소환, 초기화 등은 다른 스크립트가 관리할 것임.
    // 임시로 테스트할 수 있도록 구현
    public static TestScript Instance { get { return instance; } }
    private static TestScript instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PoolController.Instance.Init();
        MapController.Instance.LoadMap(MapUtil.LoadMap("RTD"));
        EnemyController.Instance.Init(MapController.Instance.GetMap());

        StartCoroutine(MobSpawn());
    }

    IEnumerator MobSpawn()
    {
        while(true)
        {
            int rand = Random.Range(0, EnemyManager.Keys.Count);
            int id = EnemyManager.Keys[rand];
            EnemyController.Instance.AddEnemy(id);
            yield return new WaitForSeconds(1f);
        }
    }
}
