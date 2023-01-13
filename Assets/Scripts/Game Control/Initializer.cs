using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public static Initializer Instance { get { return instance; } }
    private static Initializer instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SpriteManager.Init();

        TileManager.Init();
        EnemyManager.Init();
        TowerManager.Init();

        RoundManager.Init();
    }

    private void Start()
    {
        PoolController.Instance.Init();
        MapController.Instance.LoadMap(MapUtil.LoadMap("RTD"));
        EnemyController.Instance.Init(MapController.Instance.GetMap());
        RoundController.Instance.Init(MapController.Instance.GetMap().name);
    }
}