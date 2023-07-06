using Data;
using EnumData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class EnemyManager
{
    public static string path { get; private set; } = "/Data/Enemy/";
    private static Dictionary<int, Enemy> enemies;

    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;
    public static List<int> CustomDataKeys { get { return customDataKeys; } }
    private static List<int> customDataKeys;

    // 0: elemental, 1: grade, List: Id
    public static List<int>[,] EgEnemyIds { get { return egEnemyIds; } }
    private static List<int>[,] egEnemyIds;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static async Task GetTotal()
    {
        TotalProgress = 0;

        List<string> files = DataManager.GetFileNames(path);
        foreach (string filename in files)
        {
            List<EnemyData> list = await DataManager.DeserializeListJson<EnemyData>(path, filename);
            TotalProgress += list.Count;
        }
    }

    public static async Task Init()
    {
        enemies = new Dictionary<int, Enemy>();
        customDataKeys = new List<int>();

        egEnemyIds = new List<int>[EnumArray.Elements.Length, EnumArray.EnemyGrades.Length];
        for (int i = 0; i < EnumArray.Elements.Length; i++)
            for (int j = 0; j < EnumArray.EnemyGrades.Length; j++)
                egEnemyIds[i, j] = new List<int>();

        List<string> files = DataManager.GetFileNames(path);
        List<EnemyData> list = new List<EnemyData>();
        foreach (string filename in files)
        {
            list.AddRange(await DataManager.DeserializeListJson<EnemyData>(path, filename));
        }

        foreach (var data in list)
        {
            CurProgress++;
            Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(data);
            await SpriteManager.AddSprite<Enemy>(data.imgsrc, data.id, data.pivot, data.pixelperunit);

            Enemy enemy = new Enemy(data, anim);
            enemies.Add(enemy.id, enemy);
            egEnemyIds[(int)enemy.element, (int)enemy.grade].Add(enemy.id);

            if (data.id.ToString()[0] == '5') customDataKeys.Add(data.id);
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
            enemy.UpdateName(Translator.GetLanguage(enemy.id));
        }
    }

    public static void AddData(EnemyData data, Dictionary<AnimationType, List<Sprite>> anims)
    {
        Dictionary<AnimationType, Sprite[]> anim = new Dictionary<AnimationType, Sprite[]>();
        foreach (AnimationType key in anim.Keys)
        {
            Sprite[] sprites = new Sprite[anim[key].Length];
            for (int i = 0; i < anims[key].Count; i++)
                sprites[i] = anim[key][i];

            anim.Add(key, sprites);
        }

        Enemy newData = new Enemy(data, anim);
        if (enemies.ContainsKey(data.id))
            enemies[data.id] = newData;
        else
            enemies.Add(newData.id, newData);
    }
}