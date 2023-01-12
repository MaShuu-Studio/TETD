using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;

public static class TowerManager
{
    private static string path = Application.streamingAssetsPath + "/Data/";

    private static Dictionary<int, Tower> towers;
    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;

    public static void Init()
    {
        towers = new Dictionary<int, Tower>();
        List<TowerData> list = DataManager.DeserializeListJson<TowerData>(path, "Tower");

        foreach (var data in list)
        {
            Tower tower = new Tower(data);
            towers.Add(tower.id, tower);
        }
        keys = towers.Keys.ToList();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TOWER {towers.Count}");
#endif
    }

    public static Tower GetTower(int id)
    {
        if (towers.ContainsKey(id)) return towers[id];

        return null;
    }
}