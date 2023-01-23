using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnumData;
using Data;

public static class TowerManager
{
    private static string path = Application.streamingAssetsPath + "/Data/";

    private static Dictionary<int, Tower> towers;
    private static List<int>[,] egTowerIds;
    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;

    public static void Init()
    {
        towers = new Dictionary<int, Tower>();
        List<TowerData> list = DataManager.DeserializeListJson<TowerData>(path, "Tower");
        egTowerIds = new List<int>[EnumArray.Elements.Length, EnumArray.Grades.Length];
        for (int i = 0; i < EnumArray.Elements.Length; i++)
            for (int j = 0; j < EnumArray.Grades.Length; j++)
                egTowerIds[i, j] = new List<int>();

        foreach (var data in list)
        {
            Tower tower = new Tower(data);
            towers.Add(tower.id, tower);
            egTowerIds[(int)tower.element, (int)tower.grade].Add(tower.id);
            SpriteManager.AddSprite<Tower>(data.imgsrc, tower.id, data.pivot, data.pixelperunit);
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

    public static Tower GetRandomTower(int element, Grade grade)
    {
        if (element < 0) element = Random.Range(0, EnumArray.Elements.Length);

        int count = egTowerIds[element, (int)grade].Count;
        int rand = Random.Range(0, count);
        int id = egTowerIds[element, (int)grade][rand];

        return GetTower(id);
    }
}