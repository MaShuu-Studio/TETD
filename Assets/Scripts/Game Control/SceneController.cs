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
    = { "Title", "Game Scene", "Map Editor", "Unit Editor", "Round Editor"};

    private const string LoadingScene = "Loading";
    public bool IsLoading { get { return isLoading; } }
    private bool isLoading;
    private bool sceneLoaded;
    public string CurrentScene { get { return currentScene; } }
    private string currentScene;

    public int FindScene(string scene)
    {
        for (int i = 0; i < SceneNames.Length; i++)
        {
            if (scene == SceneNames[i]) return i;
        }
        return -1;
    }

    public async void Init()
    {
        // �ε����ȿ��� ��׶��� �۵� ���.
        Application.runInBackground = true;

        // �� Init Class���� ��ü���൵�� ����.
        await Translator.GetTotal();
        await SpriteManager.GetTotal();
        await SoundManager.GetTotal();
        await TowerManager.GetTotal();
        await EnemyManager.GetTotal();
        TileManager.GetTotal();
        await MapManager.GetTotal();
        RoundManager.GetTotal();
        CustomDataManager.GetTotal();
        UIController.GetTotal();

        int total =
             Translator.TotalProgress +
             SpriteManager.TotalProgress +
             SoundManager.TotalProgress +
             TowerManager.TotalProgress +
             EnemyManager.TotalProgress +
             TileManager.TotalProgress +
             MapManager.TotalProgress +
             RoundManager.TotalProgress +
             PoolController.TotalProgress +
             CustomDataManager.TotalProgress +
             UIController.TotalProgress;

        // �ε�â���� �ѱ�
        UIController.Instance.StartLoading();

        // �ʱ�ȭ �۾� ����.
        IEnumerator co = InitProgress(total);
        StartCoroutine(co);
        await Translator.Init();
        SpriteManager.Init();
        SoundManager.Init();

        TowerManager.Init();
        EnemyManager.Init();

        TileManager.Init();
        MapManager.Init();
        RoundManager.Init();

        // PoolController�� Tower�� Enemy�κ��� �����͸� �ޱ� ������
        // await�� �ɾ��ָ� ���� �۾��� ������ �� ��ٸ� �� ����.
        await PoolController.Instance.Init();
        await CustomDataManager.Init();
        await UIController.Instance.Init();

        StopCoroutine(co);
        ChangeScene("Title", null);
        isLoading = false;

        // �ε��� ������ ���� �ڵ����� ���� �� �ְ� ��׶��� �۵� �ɼ� ����
        Application.runInBackground = false;
    }

    private IEnumerator InitProgress(int total)
    {
        int cur = 0;
        while (total > cur)
        {
            cur =
                Translator.CurProgress +
                SpriteManager.CurProgress +
                SoundManager.CurProgress +
                TowerManager.CurProgress +
                EnemyManager.CurProgress +
                TileManager.CurProgress +
                MapManager.CurProgress +
                RoundManager.CurProgress +
                PoolController.CurProgress +
                UIController.CurProgress;

            Progress((float)cur / total, $"�����͸� �ҷ����� �� {cur} / {total}");
            yield return null;
        }
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
                    // Task�� �ƴ� Action�� ��� �ٷ� ����
                    if (actions[i].action != null)
                    {
                        actions[i].action.Invoke();
                    }
                    // Task�� ��� Func<Task>�� ��ȯ �� await
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

        //CameraController.Instance.FindCamera();
        sceneLoaded = true;
    }

    private void Progress(float value, string progress = "")
    {
        UIController.Instance.Loading(value, progress);
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
