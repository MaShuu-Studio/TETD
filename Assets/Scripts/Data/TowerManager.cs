using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnumData;
using Data;
using System.Threading.Tasks;

public static class TowerManager
{
    public static string path { get; private set; } = "/Data/Tower/";

    public static bool isLoaded { get; private set; }

    private static Dictionary<int, Tower> towers;
    public static Dictionary<int, Sprite[]> Projs { get { return projectiles; } }
    private static Dictionary<int, Sprite[]> projectiles;

    public static Dictionary<int, Sprite[]> Effects { get { return effects; } }
    private static Dictionary<int, Sprite[]> effects;

    // 0: elemental, 1: grade, List: Id
    public static List<int>[,] EgTowerIds { get { return egTowerIds; } }
    private static List<int>[,] egTowerIds;

    public static List<int>[] AbilityIds { get { return abilityIds; } }
    private static List<int>[] abilityIds;

    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;

    private static int originDataAmount;
    private static int[,] customDataIndexes;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static async Task GetTotal()
    {
        TotalProgress = 0;
        List<string> files = DataManager.GetFileNames(path);
        foreach (string filename in files)
        {
            List<TowerData> list = await DataManager.DeserializeListJson<TowerData>(path, filename);
            TotalProgress += list.Count;
        }
    }

    public static async void Init()
    {
        isLoaded = false;

        keys = new List<int>();
        towers = new Dictionary<int, Tower>();
        projectiles = new Dictionary<int, Sprite[]>();
        effects = new Dictionary<int, Sprite[]>();

        egTowerIds = new List<int>[EnumArray.Elements.Length, EnumArray.Grades.Length];

        for (int i = 0; i < EnumArray.Elements.Length; i++)
            for (int j = 0; j < EnumArray.Grades.Length; j++)
                egTowerIds[i, j] = new List<int>();

        abilityIds = new List<int>[(int)EnumArray.AbilityTypes[EnumArray.AbilityTypes.Length - 1] + 1];
        for (int i = 0; i < abilityIds.Length; i++)
            abilityIds[i] = new List<int>();

        List<string> files = DataManager.GetFileNames(path);
        List<TowerData> list = new List<TowerData>();
        foreach (string filename in files)
        {
            list.AddRange(await DataManager.DeserializeListJson<TowerData>(path, filename));
        }

        foreach (var data in list)
        {
            AddData(data.id, data);
        }
        
        while (keys.Count < list.Count) await Task.Yield();
        isLoaded = true;

        originDataAmount = towers.Count;
        customDataIndexes = new int[EnumArray.Elements.Length, EnumArray.Grades.Length];

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD TOWER {towers.Count}");
#endif
    }

    private static async void AddData(int id, TowerData data)
    {
        Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(data);
        Tower tower = new Tower(data, anim);

        await SpriteManager.AddSprite<Tower>(data.imgsrc, id, data.pivot, data.pixelperunit);

        Sprite[] effect = await MakeObjects(data, "EFFECT");
        if (effect != null) effects.Add(id, effect);

        Sprite[] proj = await MakeObjects(data, "WEAPON");
        if (proj != null) projectiles.Add(id, proj);

        keys.Add(id);
        towers.Add(id, tower);
        egTowerIds[(int)tower.element, (int)tower.grade].Add(id);

        if (tower.AbilityTypes != null)
            for (int i = 0; i < tower.AbilityTypes.Length; i++)
            {
                int type = (int)tower.AbilityTypes[i];
                abilityIds[type].Add(id);
            }

        CurProgress++;
    }

    public static async Task<Dictionary<AnimationType, Sprite[]>> MakeAnimation(TowerData data)
    {
        Dictionary<AnimationType, Sprite[]> anim = new Dictionary<AnimationType, Sprite[]>();

        for (int i = 0; i < EnumArray.AnimationTypes.Length; i++)
        {
            AnimationType type = EnumArray.AnimationTypes[i];
            string animationName = EnumArray.AnimationTypeStrings[type];
            List<Sprite> sprites = new List<Sprite>();
            while (true)
            {
                // IDLE0.png �� ���� ���
                string filename = animationName + sprites.Count + ".png";
                Sprite sprite = await DataManager.LoadSprite(data.imgsrc + filename, data.pivot, data.pixelperunit);
                if (sprite == null) break; // �̹����� ���ٸ� �н�
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

    public static async Task<Sprite[]> MakeObjects(TowerData data, string type)
    {
        // ����ü�� �̸��� WEAPON*
        // ����Ʈ�� �̸��� EFFECT*
        Sprite[] s = null;

        List<Sprite> sprites = new List<Sprite>();
        while (true)
        {
            // IDLE0.png �� ���� ���
            string filename = type + sprites.Count + ".png";
            Sprite sprite = await DataManager.LoadSprite(data.imgsrc + filename, data.pivot, data.pixelperunit);
            if (sprite == null) break; // �̹����� ���ٸ� �н�
            sprites.Add(sprite);
        }

        if (sprites.Count > 0)
        {
            s = new Sprite[sprites.Count];
            sprites.CopyTo(s);
        }

        return s;
    }

    public static Tower GetTower(int id)
    {
        if (towers.ContainsKey(id)) return towers[id];

        return null;
    }

    public static void UpdateLanguage(LanguageType type)
    {
        foreach (var tower in towers.Values)
        {
            tower.UpdateName(Translator.GetLanguage(tower.id));
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

            // id�� AEEGNNN���� �Ǿ�����.
            int element, grade;

            element = id % 1000000; // A ����
            element = element / 10000; // GNNN ����

            grade = id % 10000; // AEE ����
            grade = grade / 1000; // NNN ����


            RemoveData(id, element, grade);
        }
    }

    public static async void LoadCustomData(List<string> pathes)
    {
        if (pathes == null)
        {
            TotalProgress = 0;
            return;
        }

        // CustomData�� index������ element, grade �� 1000���� ��Ƽ� Ȱ��. 0~999
        // �̸� �Ѵ´ٸ� ���̻� �߰��� �� ������ ��. ���� �ڵ� ���� �ʿ�.

        List<TowerData> list = new List<TowerData>();
        foreach (string path in pathes)
        {
            list.AddRange(await DataManager.DeserializeListJson<TowerData>(path));
        }
        TotalProgress = list.Count;

        foreach (var data in list)
        {
            // �ӽ÷� id�� ���� element�� grade�� ���� Ȯ����.
            // ���� ���ֿ����͸� �����ϸ� id�� ���������ʰ� element�� grade�� ���� �����ϰ� �� ���̸�
            // �̸� ���� �ڵ����� ���̵� ������ �� �ֵ��� ��.
            // ����, Sprite ���� �ε� ����� �ٲ�� ��. �ش� �κп� ���� ����� �ʿ��� ��.
            // �켱�� ���� id�� ���� �޾Ƽ� Ȱ���� �� �ֵ��� ��.
            int originId = data.id;

            // id�� AEEGNNN���� �Ǿ�����.
            int element, grade;

            element = originId % 1000000; // A ����
            element = element / 10000; // GNNN ����

            grade = originId % 10000; // AEE ����
            grade = grade / 1000; // NNN ����

            int id = 4000000 + element * 10000 + grade * 1000 + (customDataIndexes[element, grade]++);

            AddData(id, data);
        }
    }

    /*
    public static void AddData(TowerData data, int element, int grade,
        Dictionary<AnimationType, List<Sprite>> anims, Dictionary<string, List<Sprite>> efprojs)
    {
        Dictionary<AnimationType, Sprite[]> anim = new Dictionary<AnimationType, Sprite[]>();
        foreach (var key in anims.Keys)
        {
            Sprite[] sprites = new Sprite[anims[key].Count];
            for (int i = 0; i < anims[key].Count; i++)
                sprites[i] = anims[key][i];

            anim.Add(key, sprites);
        }

        Tower newData = new Tower(data, anim);

        if (efprojs.Count > 0)
        {
            Dictionary<string, Sprite[]> epanim = new Dictionary<string, Sprite[]>();
            foreach (var key in efprojs.Keys)
            {
                Sprite[] sprites = new Sprite[efprojs[key].Count];
                for (int i = 0; i < efprojs[key].Count; i++)
                    sprites[i] = efprojs[key][i];

                epanim.Add(key, sprites);
            }
            if (towers.ContainsKey(newData.id))
            {
                effects[newData.id] = epanim["EFFECT"];
                projectiles[newData.id] = epanim["WEAPON"];
            }
            else
            {
                effects.Add(newData.id, epanim["EFFECT"]);
                projectiles.Add(newData.id, epanim["WEAPON"]);
            }
        }

        if (towers.ContainsKey(newData.id))
            towers[newData.id] = newData;
        else
        {
            towers.Add(newData.id, newData);
            keys.Add(newData.id);
            egTowerIds[element, grade].Add(newData.id);
        }
    }*/

    public static void RemoveData(int id, int element, int grade)
    {
        if (towers.ContainsKey(id))
        {
            towers.Remove(id);
            effects.Remove(id);
            projectiles.Remove(id);
            keys.Remove(id);
            egTowerIds[element, grade].Remove(id);
            for (int i = 0; i < abilityIds.Length; i++)
            {
                if (abilityIds[i].Contains(id))
                    abilityIds[i].Remove(id);
            }    

            SpriteManager.RemoveData(id);
        }
    }
}