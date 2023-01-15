using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    = { "Title", "Game Scene" };

    private const string LoadingScene = "Loading";
    public bool IsLoaded { get { return isLoaded; } }
    private bool isLoaded;
    private string currentScene;
    private IEnumerator loadCoroutine;

    public void Init()
    {
        ChangeScene("Title");
    }

    public int FindScene(string scene)
    {
        for (int i = 0; i < SceneNames.Length; i++)
        {
            if (scene == SceneNames[i]) return i;
        }
        return -1;
    }

    public void ChangeScene(string scene)
    {
        int sceneNumber = FindScene(scene);
        if (loadCoroutine != null || sceneNumber == -1) return;

        isLoaded = true;
        Progress(0);
        UIController.Instance.StartLoading();

        loadCoroutine = LoadScene(sceneNumber);
        StartCoroutine(loadCoroutine);
    }

    private IEnumerator LoadScene(int sceneNumber)
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
            Progress(async.progress);
            yield return null;
        }
        isLoaded = false;

        UIController.Instance.ChangeScene(sceneNumber);
        currentScene = scene;

        loadCoroutine = null;
    }

    private void Progress(float value)
    {
        UIController.Instance.Loading(value);
    }
}
