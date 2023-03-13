using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;
using EnumData;
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
            Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(data);
            Enemy enemy = new Enemy(data, anim);
            enemies.Add(enemy.id, enemy);
            await SpriteManager.AddSprite<Enemy>(data.imgsrc, enemy.id, data.pivot, data.pixelperunit);
        }

        keys = enemies.Keys.ToList();
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD ENEMY {enemies.Count}");
#endif
    }
    private static async Task<Dictionary<AnimationType, Sprite[]>> MakeAnimation(EnemyData data)
    {
        Dictionary<AnimationType, Sprite[]> anim = new Dictionary<AnimationType, Sprite[]>();

        for (int i = 0; i < EnumArray.AnimationTypes.Length; i++)
        {
            AnimationType type = EnumArray.AnimationTypes[i];
            string animationName = EnumArray.AnimationTypeStrings[type];
            List<Sprite> sprites = new List<Sprite>();
            while (true)
            {
                // IDLE0.png 와 같은 방식
                string filename = animationName + sprites.Count + ".png";
                Sprite sprite = await DataManager.LoadSprite(data.imgsrc + filename, data.pivot, data.pixelperunit);
                if (sprite == null) break; // 이미지가 없다면 패스
                sprites.Add(sprite);
            }

            if (sprites.Count > 0)
            {
                Sprite[] s = new Sprite[sprites.Count];
                sprites.CopyTo(s);

                anim.Add(type, s);
            }
        }

        return anim;
    }

    public static Enemy GetEnemy(int id)
    {
        if (enemies.ContainsKey(id)) return enemies[id];

        return null;
    }
    public static void UpdateLanguage(LanguageType type)
    {
        foreach (var enemy in enemies.Values)
        {
            enemy.UpdateName(Translator.GetLanguage(enemy.id), type);
        }
    }
}