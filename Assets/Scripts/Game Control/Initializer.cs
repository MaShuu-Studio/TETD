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

#if UNITY_EDITOR
        Data.DataManager.MakeFileNameList();
#endif
    }

    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        List<SceneAction> actions = new List<SceneAction>();

        actions.Add(new SceneAction(() => EnumData.EnumArray.Init()));

        actions.Add(new SceneAction(SpriteManager.Init()));
        actions.Add(new SceneAction(SoundManager.Init()));

        actions.Add(new SceneAction(TileManager.Init()));
        actions.Add(new SceneAction(EnemyManager.Init()));
        actions.Add(new SceneAction(TowerManager.Init()));

        actions.Add(new SceneAction(MapManager.Init()));
        actions.Add(new SceneAction(RoundManager.Init()));

        actions.Add(new SceneAction(() => PoolController.Instance.Init()));
        SceneController.Instance.ChangeScene("Title", actions);
    }
}