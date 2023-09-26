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
        paused = false;
    }
    public bool Paused { get { return paused || tutorialPaused; } }
    private bool paused;
    private bool tutorialPaused;

    private void Update()
    {
        if (SceneController.Instance.IsLoading == false)
        {
            bool esc = Input.GetButtonDown("Cancel");
            if (esc) Pause(!paused);

            if (SceneController.Instance.CurrentScene == "Game Scene")
            {
                if (PlayerController.Instance.Life <= 0)
                {
                    GameOver();
                }
                else if (RoundController.Instance.IsEnd && EnemyController.Instance.EnemyAmount == 0)
                {
                    Clear();
                }
            }
        }
    }

    public void Title()
    {
        if (SceneController.Instance.CurrentScene == "Title") return;

        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(() => UIController.Instance.Title()));
        SceneController.Instance.ChangeScene("Title", actions);

        paused = false;
        UIController.Instance.OpenSetting(paused);

        // 임시 처리 방식
        TowerManager.ResetCustomData();
        EnemyManager.ResetCustomData();
        MapManager.ResetCustomData();
    }

    public void StartGame()
    {
        CharacterType character;
        List<DifficultyType> difficulties;
        string mapName;
        UIController.Instance.GameSetting(out character, out difficulties, out mapName);

        Map map = MapManager.LoadMap(mapName);
        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(() => MapController.Instance.Init(map)));
        actions.Add(new SceneAction(() => EnemyController.Instance.Init(map, difficulties)));
        actions.Add(new SceneAction(() => RoundController.Instance.Init(map.name, difficulties)));
        actions.Add(new SceneAction(() => PlayerController.Instance.Init(character, difficulties)));
        actions.Add(new SceneAction(() => TowerController.Instance.Init()));
        actions.Add(new SceneAction(() => UIController.Instance.StartGame()));

        SceneController.Instance.ChangeScene("Game Scene", actions);
    }

    public void EndEditor()
    {/*
        TowerManager.ResetCustomData();
        EnemyManager.ResetCustomData();
        MapManager.ResetCustomData();*/

        Title();
    }

    public void EditCustomData(int index, List<string>[] pathes)
    {
        SceneController.Instance.EditCustomData(index, pathes);
    }

    public void EditMap()
    {
        string mapName = UIController.Instance.GetMapName();
        Map map = UIController.Instance.GetMap();

        MapEditor.Instance.Init(map, mapName);
        UIController.Instance.EditMap(map);
    }

    public void EditRound()
    {

    }

    public void Clear()
    {
        UIController.Instance.Clear();
    }

    public void GameOver()
    {
        UIController.Instance.GameOver();
    }

    public void Pause(bool b)
    {
        paused = b;
        UIController.Instance.OpenSetting(paused);
    }
    public void TutorialPause(bool b)
    {
        tutorialPaused = b;
    }

    private void OnApplicationPause(bool pause)
    {
        if (SceneController.Instance.CurrentScene == "Game Scene" && pause)
            Pause(true);
    }
}
