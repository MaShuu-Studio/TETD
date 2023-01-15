using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;

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

    private IEnumerator loadingCoroutine;
    
    public void StartGame(string mapName)
    {
        if (loadingCoroutine != null) return;
        SceneController.Instance.ChangeScene("Game Scene");
        loadingCoroutine = LoadGame(mapName);
        StartCoroutine(loadingCoroutine);
    }

    IEnumerator LoadGame(string mapName)
    {
        while (SceneController.Instance.IsLoaded) yield return null;

        Map map = MapManager.LoadMap(mapName);
        if (map != null)
        {
            MapController.Instance.Init(map);
            EnemyController.Instance.Init(map);
            RoundController.Instance.Init(map.name);
            PlayerController.Instance.Init();
            UIController.Instance.StartGame();
        }
        loadingCoroutine = null;
    }
}
