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
    private static string path = Application.streamingAssetsPath + "/Sounds/";

    public static async Task Init()
    {
        sounds = new Dictionary<string, AudioClip>();

       // List<string> names = await DataManager.GetFiles(path, ".wav");
        List<string> names = new List<string>()
        {
            "1001",
            "1002",
            "1003",
            "1004",
            "1005",
            "1006",
            "1007",
            "1008",
            "1009",
            "1010",
            "1011",
            "1012"
        };
        foreach (string name in names)
        {
            AudioClip clip = await DataManager.LoadSound("/" + name + ".wav", name, AudioType.WAV);
            sounds.Add(name, clip);
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
