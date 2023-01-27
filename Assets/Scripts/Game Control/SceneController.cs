using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get { return instance; } }
    private static SceneController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private string[] SceneNames { get; }
    = { "Title", "Game Scene", "Map Editor" };

    private const string LoadingScene = "Loading";
    private bool isLoading;
    private bool sceneLoaded;
    private string currentScene;

    public int FindScene(string scene)
    {
        for (int i = 0; i < SceneNames.Length; i++)
        {
            if (scene == SceneNames[i]) return i;
        }
        return -1;
    }

    public async void ChangeScene(string scene, List<SceneAction> actions)
    {
        int sceneNumber = FindScene(scene);
        if ((isLoading == true || sceneNumber == -1) == false)
        {
            isLoading = true;
            Progress(0);
            UIController.Instance.StartLoading();

            int count = 1;
            if (actions != null) count += actions.Count;
            sceneLoaded = false;
            StartCoroutine(LoadScene(sceneNumber, count));

            while (sceneLoaded == false) await Task.Yield();
            if (actions != null)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    // Task가 아닌 Action의 경우 바로 진행
                    if (actions[i].action != null)
                    {
                        actions[i].action.Invoke();
                    }
                    // Task의 경우 Func<Task>로 변환 및 await
                    else
                    {
                        Func<Task> task = async () => await actions[i].task;
                        await task.Invoke();
                    }
                    Progress((1 + i + 1) / (float)count);
                }
            }

            UIController.Instance.ChangeScene(sceneNumber);
            currentScene = scene;
            isLoading = false;
        }
    }

    private IEnumerator LoadScene(int sceneNumber, int count)
    {
        string scene = SceneNames[sceneNumber];
        AsyncOperation async = SceneManager.LoadSceneAsync(LoadingScene);

        while (async.isDone == false)
        {
            yield return null;
        }

        async = SceneManager.LoadSceneAsync(scene);

        while (async.isDone == false)
        {
            Progress(async.progress / (float)count);
            yield return null;
        }
        sceneLoaded = true;
    }

    private void Progress(float value)
    {
        UIController.Instance.Loading(value);
    }
}

public class SceneAction
{
    public Action action;
    public Task task;

    public SceneAction(Action action)
    {
        this.action = action;
        task = null;
    }
    public SceneAction(Task task)
    {
        this.action = null;
        this.task = task;
    }
}
