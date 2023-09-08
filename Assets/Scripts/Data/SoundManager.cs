using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Threading.Tasks;

public static class SoundManager
{
    private static Dictionary<string, AudioClip> sounds;
    public static List<string> Keys { get { return keys; } }
    private static List<string> keys;
    private static string path = "/Sounds/";
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static void GetTotal()
    {
        List<string> fileNames = DataManager.GetFileNames(path);
        TotalProgress = fileNames.Count;

        sounds = new Dictionary<string, AudioClip>();
    }

    public static async Task Init()
    {
        List<string> fileNames = DataManager.GetFileNames(path);

        for (int i = 0; i < fileNames.Count; i++)
        {
            CurProgress++;
            string filePath = path + fileNames[i];
            AudioClip clip = await DataManager.LoadSound(filePath, AudioType.WAV);
            sounds.Add(DataManager.FileNameTriming(fileNames[i]).ToUpper(), clip);
        }
        keys = sounds.Keys.ToList();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD SOUND {sounds.Count}");
#endif

        SoundController.Instance.Init();
    }

    public static AudioClip GetSound(string name)
    {
        name = name.ToUpper();

        if (sounds.ContainsKey(name)) return sounds[name];
        return null;
    }
}
