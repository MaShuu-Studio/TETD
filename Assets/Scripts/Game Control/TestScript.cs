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

        StartCoroutine(MobSpawn("ENEMY-TEST"));
    }

    IEnumerator MobSpawn(string name)
    {
        while(true)
        {
            GameObject go = PoolController.Pop(name);
            EnemyObject enemy = go.GetComponent<EnemyObject>();
            EnemyController.Instance.AddEnemy(enemy);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
