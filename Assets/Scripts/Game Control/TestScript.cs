using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // �Ŀ� ���� ��ȯ, �ʱ�ȭ ���� �ٸ� ��ũ��Ʈ�� ������ ����.
    // �ӽ÷� �׽�Ʈ�� �� �ֵ��� ����
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
            yield return new WaitForSeconds(0.1f);
        }
    }
}
