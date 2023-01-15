using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Data;

public static class RoundManager
{
    private static string path = Application.streamingAssetsPath + "/Data/";

    private static Dictionary<string, Round> rounds;
    public static List<string> Keys { get { return keys; } }
    private static List<string> keys;

    public static void Init()
    {
        rounds = new Dictionary<string, Round>();
        List<RoundData> list = DataManager.DeserializeListJson<RoundData>(path, "Round");


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
    }
}
