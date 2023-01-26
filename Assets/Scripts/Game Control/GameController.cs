using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;
using System.Threading.Tasks;

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

        Map map = MapManager.LoadMap(mapName);
        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(() => MapController.Instance.Init(map)));
        actions.Add(new SceneAction(() => EnemyController.Instance.Init(map)));
        actions.Add(new SceneAction(() => RoundController.Instance.Init(map.name)));
        actions.Add(new SceneAction(() => PlayerController.Instance.Init(character)));
        actions.Add(new SceneAction(() => TowerController.Instance.Init()));
        actions.Add(new SceneAction(() => UIController.Instance.StartGame()));

        SceneController.Instance.ChangeScene("Game Scene", actions);
    }
}
