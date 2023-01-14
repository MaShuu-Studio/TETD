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

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image cutSceneImage;
    [SerializeField] private Slider loadingGage;

    private const string LoadingScene = "Loading";
    private string currentScene;
    private IEnumerator loadCoroutine;

    public void Init()
    {
        ChangeScene("Title");
    }

    public void ChangeScene(string scene)
    {
        if (loadCoroutine != null) return;

        loadingGage.value = 0;
        canvas.SetActive(true);

        loadCoroutine = LoadScene(scene);
        StartCoroutine(loadCoroutine);
    }

    private IEnumerator LoadScene(string scene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(LoadingScene);

        while (async.isDone == false)
        {
            yield return null;
        }

        async = SceneManager.LoadSceneAsync(scene);

        while (async.isDone == false)
        {
            loadingGage.value = async.progress;
            yield return null;
        }

        canvas.SetActive(false);
        currentScene = scene;

        loadCoroutine = null;
    }
}
