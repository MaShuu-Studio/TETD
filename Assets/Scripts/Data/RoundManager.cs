using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Data;
using System.Threading.Tasks;

public static class RoundManager
{
    private static string path = "/Data/";

    private static Round round;
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static void GetTotal()
    {
        TotalProgress = 1;
    }

    public static async void Init()
    {
        CurProgress++;
        RoundData data = await DataManager.DeserializeJson<RoundData>(path, "Round");
        round = new Round(data);

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD ROUND DATA");
#endif
    }

    public static Round GetRound(string mapName)
    {
        return round;
    }

    /*
    private static Dictionary<string, Round> rounds;
    public static List<string> Keys { get { return keys; } }
    private static List<string> keys;

    public static async Task Init()
    {
        rounds = new Dictionary<string, Round>();
        List<RoundData> list = await DataManager.DeserializeListJson<RoundData>(path, "Round");

        foreach (var data in list)
        {
            Round round = new Round(data);
            if (MapManager.Maps.Contains(data.mapName.ToUpper()) == false) continue;
            rounds.Add(data.mapName, round);
        }

        keys = rounds.Keys.ToList();
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD ROUND {rounds.Count}");
#endif
    }
    
    public static Round GetRound(string mapName)
    {
        if (rounds.ContainsKey(mapName)) return rounds[mapName];

        return null;
    }*/
}
