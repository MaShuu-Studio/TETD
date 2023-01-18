using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

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

    private CharacterType character;
    private List<DifficultyType> difficulties;
    private string mapName;
    
    public void SettingGame(CharacterType c, List<DifficultyType> diff, string map)
    {
        character = c;
        difficulties = diff;
        mapName = map;
    }

    public void StartGame()
    {
        if (loadingCoroutine != null) return;
        UIController.Instance.SettingGame();

        SceneController.Instance.ChangeScene("Game Scene");
        loadingCoroutine = LoadGame();
        StartCoroutine(loadingCoroutine);
    }

    IEnumerator LoadGame()
    {
        while (SceneController.Instance.IsLoaded) yield return null;

        Map map = MapManager.LoadMap(mapName);
        if (map != null)
        {
            MapController.Instance.Init(map);
            EnemyController.Instance.Init(map);
            RoundController.Instance.Init(map.name);
            PlayerController.Instance.Init(character);
            UIController.Instance.StartGame();
        }
        loadingCoroutine = null;
    }
}
