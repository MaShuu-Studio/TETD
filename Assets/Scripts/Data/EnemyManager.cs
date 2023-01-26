using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;
using System.Threading.Tasks;

public static class EnemyManager
{
    private static string path = Application.streamingAssetsPath + "/Data/";

    private static Dictionary<int, Enemy> enemies;
    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;

    public static async Task Init()
    {
        enemies = new Dictionary<int, Enemy>();
        List<EnemyData> list = await DataManager.DeserializeListJson<EnemyData>(path, "Enemy");

        foreach (var data in list)
        {
            Enemy enemy = new Enemy(data);
            enemies.Add(enemy.id, enemy);
            await SpriteManager.AddSprite<Enemy>(data.imgsrc, enemy.id, data.pivot, data.pixelperunit);
        }

        keys = enemies.Keys.ToList();
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD ENEMY {enemies.Count}");
#endif
    }

    public static Enemy GetEnemy(int id)
    {
        if (enemies.ContainsKey(id)) return enemies[id];

        return null;
    }
}