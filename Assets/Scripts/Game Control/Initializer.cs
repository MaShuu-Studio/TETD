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

        Data.DataManager.MakeFileNameList();
        EnumData.EnumArray.Init();

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        List<SceneAction> actions = new List<SceneAction>();

        actions.Add(new SceneAction(Translator.Init()));
        actions.Add(new SceneAction(SpriteManager.Init()));
        actions.Add(new SceneAction(SoundManager.Init()));

        actions.Add(new SceneAction(TileManager.Init()));
        actions.Add(new SceneAction(EnemyManager.Init()));
        actions.Add(new SceneAction(TowerManager.Init()));

        actions.Add(new SceneAction(MapManager.Init()));
        actions.Add(new SceneAction(RoundManager.Init()));

        actions.Add(new SceneAction(() => PoolController.Instance.Init()));
        actions.Add(new SceneAction(() => GameController.Instance.SetLanguage(0)));
        actions.Add(new SceneAction(UIController.Instance.Init()));
        SceneController.Instance.ChangeScene("Title", actions);
    }
}