using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogCollector : MonoBehaviour
{
    public static LogCollector Instance { get { return instance; } }
    private static LogCollector instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Application.logMessageReceived += HandleLog;

        DontDestroyOnLoad(gameObject);
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            string str =
                "Log : " + logString +
                "\nTrace : " + stackTrace +
                "\nType : " + type.ToString();

            UIController.Instance.ErrorLog(logString, str);
        }
    }
}
