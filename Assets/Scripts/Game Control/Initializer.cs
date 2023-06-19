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
        SceneController.Instance.Init();
    }
}