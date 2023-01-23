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

        Initialize();
    }

    private async void Initialize()
    {
        SpriteManager.Init();
        await SoundManager.Init();

        await TileManager.Init();
        await EnemyManager.Init();
        await TowerManager.Init();

        MapManager.Init();
        RoundManager.Init();

        PoolController.Instance.Init();
        SceneController.Instance.Init();
    }
}