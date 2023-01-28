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
        settingUI = false;
    }

    private bool settingUI;
    private void Update()
    {
        if(SceneController.Instance.IsLoading == false)
        {
            bool esc = Input.GetButtonDown("Cancel");
            if (esc)
            {
                settingUI = !settingUI;
                UIController.Instance.OpenSetting(settingUI);
            }
        }
    }

    public void Title()
    {
        if (SceneController.Instance.CurrentScene == "Title") return;

        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(UIController.Instance.Title()));
        SceneController.Instance.ChangeScene("Title", actions);

        settingUI = false;
        UIController.Instance.OpenSetting(settingUI);
    }

    public async void StartGame()
    {
        CharacterType character;
        List<DifficultyType> difficulties;
        string mapName;
        UIController.Instance.GameSetting(out character, out difficulties, out mapName);

        Map map = await MapManager.LoadMap(mapName);
        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(() => MapController.Instance.Init(map)));
        actions.Add(new SceneAction(() => EnemyController.Instance.Init(map, difficulties)));
        actions.Add(new SceneAction(() => RoundController.Instance.Init(map.name, difficulties)));
        actions.Add(new SceneAction(() => PlayerController.Instance.Init(character, difficulties)));
        actions.Add(new SceneAction(() => TowerController.Instance.Init()));
        actions.Add(new SceneAction(() => UIController.Instance.StartGame()));

        SceneController.Instance.ChangeScene("Game Scene", actions);
    }

    public async void EditMap()
    {
        string mapName = UIController.Instance.GetMapName();
        if (string.IsNullOrEmpty(mapName)) return;

        Map map = await MapManager.LoadMap(mapName);
        List<SceneAction> actions = new List<SceneAction>();
        actions.Add(new SceneAction(() => MapEditor.Instance.Init(map, mapName)));
        actions.Add(new SceneAction(() => UIController.Instance.EditMap(mapName)));
        SceneController.Instance.ChangeScene("Map Editor", actions);
    }
}
