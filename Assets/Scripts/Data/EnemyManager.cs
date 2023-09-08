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

    private static int originDataAmount;
    private static int[,] customDataIndexes;
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
        }

        keys = enemies.Keys.ToList();

        originDataAmount = enemies.Count;
        customDataIndexes = new int[EnumArray.Elements.Length, EnumArray.Grades.Length];

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

    public static void AddData(EnemyData data, int element, int grade, Dictionary<AnimationType, List<Sprite>> anims)
    {
        Dictionary<AnimationType, Sprite[]> anim = new Dictionary<AnimationType, Sprite[]>();
        foreach (AnimationType key in anims.Keys)
        {
            Sprite[] sprites = new Sprite[anims[key].Count];
            for (int i = 0; i < anims[key].Count; i++)
                sprites[i] = anims[key][i];

            anim.Add(key, sprites);
        }

        Enemy newData = new Enemy(data, anim);
        if (enemies.ContainsKey(newData.id))
            enemies[newData.id] = newData;
        else
        {
            enemies.Add(newData.id, newData);
            keys.Add(newData.id);
            customDataKeys.Add(newData.id);
            egEnemyIds[element, grade].Add(newData.id);
        }
    }
    public static void RemoveData(int id, int element, int grade)
    {
        if (enemies.ContainsKey(id))
        {
            enemies.Remove(id);
            keys.Remove(id);
            customDataKeys.Remove(id);
            egEnemyIds[element, grade].Remove(id);
        }
    }
    public static void ResetCustomData()
    {
        CurProgress = 0;
        TotalProgress = 9999;

        for (int i = 0; i < customDataIndexes.GetLength(0); i++)
        {
            for (int j = 0; j < customDataIndexes.GetLength(1); j++)
            {
                customDataIndexes[i, j] = 0;
            }
        }

        while (keys.Count > originDataAmount)
        {
            int index = keys.Count - 1;
            int id = keys[index];

            // id는 AEEGNNN으로 되어있음.
            int element, grade;

            element = id % 1000000; // A 제거
            element = element / 10000; // GNNN 제거

            grade = id % 10000; // AEE 제거
            grade = grade / 1000; // NNN 제거


            RemoveData(id, element, grade);
        }
    }

    public static async Task LoadCustomData(List<string> pathes)
    {
        // CustomData의 index범위를 element, grade 별 1000으로 잡아서 활용. 0~999
        // 이를 넘는다면 더이상 추가할 수 없도록 함. 관련 코드 기입 필요.

        List<EnemyData> list = new List<EnemyData>();
        foreach (string path in pathes)
        {
            list.AddRange(await DataManager.DeserializeListJson<EnemyData>(path));
        }
        TotalProgress = list.Count;

        foreach (var data in list)
        {
            // 임시로 id를 통해 element와 grade를 직접 확인함.
            // 추후 유닛에디터를 수정하며 id를 기입하지않고 element와 grade를 따로 저장하게 할 것이며
            // 이를 통해 자동으로 아이디를 생성할 수 있도록 함.
            // 또한, Sprite 역시 로드 방식을 바꿔야 함. 해당 부분에 대한 고민이 필요할 듯.
            // 우선은 기존 id로 부터 받아서 활용할 수 있도록 함.
            int originId = data.id;

            // id는 AEEGNNN으로 되어있음.
            int element, grade;

            element = originId % 1000000; // A 제거
            element = element / 10000; // GNNN 제거

            grade = originId % 10000; // AEE 제거
            grade = grade / 1000; // NNN 제거


            int id = 5000000 + element * 10000 + grade * 1000 + (customDataIndexes[element, grade]++);

            Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(data);
            await SpriteManager.AddSprite<Enemy>(data.imgsrc, id, data.pivot, data.pixelperunit);

            Enemy enemy = new Enemy(data, anim);
            enemies.Add(id, enemy);
            egEnemyIds[(int)enemy.element, (int)enemy.grade].Add(id);

            CurProgress++;
        }
    }
}