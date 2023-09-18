using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using EnumData;
using System.Threading.Tasks;

public static class CustomDataManager
{
    private static string path = "/Data/Custom/";
    public static string editingPath { get { return path + editingDataName + "/"; } }
    public static string editingTowerSpritePath { get; private set; }
    public static string editingEnemySpritePath { get; private set; }
    public static string editingMapPath { get; private set; }
    public static string editingLangPath { get; private set; }
    public static List<CustomData> Datas { get { return datas; } }
    private static List<CustomData> datas;

    private static int editingDataIndex;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static string EditingDataName { get { return editingDataName; } }
    private static string editingDataName;

    public static List<int> TowerKeys { get { return towerKeys; } }
    private static List<int> towerKeys;
    public static List<int> EnemyKeys { get { return enemyKeys; } }
    private static List<int> enemyKeys;

    public static Dictionary<int, Tower> EditingTowerData { get { return editingTowerData; } }
    private static Dictionary<int, Tower> editingTowerData;
    public static Dictionary<int, Sprite> EditingTowerSprites { get { return editingTowerSprites; } }
    private static Dictionary<int, Sprite> editingTowerSprites;
    public static Dictionary<int, Sprite[]> EditingTowerEffects { get { return editingTowerEffects; } }
    private static Dictionary<int, Sprite[]> editingTowerEffects;
    public static Dictionary<int, Sprite[]> EditingTowerProjs { get { return editingTowerProjs; } }
    private static Dictionary<int, Sprite[]> editingTowerProjs;

    public static Dictionary<int, Sprite> EditingEnemySprites { get { return editingEnemySprites; } }
    private static Dictionary<int, Sprite> editingEnemySprites;
    public static Dictionary<int, Enemy> EditingEnemyData { get { return editingEnemyData; } }
    private static Dictionary<int, Enemy> editingEnemyData;

    public static List<string> EditingMapNames { get { return editingMapNames; } }
    private static List<string> editingMapNames;
    public static Dictionary<string, Map> EditingMapData { get { return editingMapData; } }
    private static Dictionary<string, Map> editingMapData;

    public static void GetTotal()
    {
        TotalProgress = 1;
    }

    public static async Task Init()
    {
        datas = await DataManager.LoadCustomDataList(path);
        CurProgress += 1;

        towerKeys = new List<int>();
        editingTowerData = new Dictionary<int, Tower>();
        editingTowerSprites = new Dictionary<int, Sprite>();
        editingTowerEffects = new Dictionary<int, Sprite[]>();
        editingTowerProjs = new Dictionary<int, Sprite[]>();

        enemyKeys = new List<int>();
        editingEnemyData = new Dictionary<int, Enemy>();
        editingEnemySprites = new Dictionary<int, Sprite>();
        editingMapNames = new List<string>();
        editingMapData = new Dictionary<string, Map>();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD CUSTOM DATA LIST");
#endif
    }

    public static async void LoadCustomData(int index, List<string>[] pathes)
    {
        editingDataIndex = index;
        editingDataName = datas[index].name;
        editingLangPath = path + editingDataName + "/Language/";
        editingMapPath = Application.streamingAssetsPath + path + editingDataName + "/Map/";
        datas.FindIndex(data => data.name == editingDataName);

        CurProgress = 0;
        TotalProgress = 9999;

        towerKeys.Clear();
        editingTowerData.Clear();
        editingTowerSprites.Clear();
        editingTowerEffects.Clear();
        editingTowerProjs.Clear();

        enemyKeys.Clear();
        editingEnemyData.Clear();
        editingEnemySprites.Clear();
        editingMapNames.Clear();
        editingMapData.Clear();

        if (pathes == null)
        {
            TotalProgress = 0;
            return;
        }

        // CustomData의 index범위를 element, grade 별 1000으로 잡아서 활용. 0~999
        // 이를 넘는다면 더이상 추가할 수 없도록 함. 관련 코드 기입 필요.

        List<TowerData> towerList = new List<TowerData>();
        List<EnemyData> enemyList = new List<EnemyData>();

        if (pathes[0] != null)
            foreach (string path in pathes[0])
            {
                towerList.AddRange(await DataManager.DeserializeListJson<TowerData>(path));
            }
        TotalProgress = towerList.Count;

        if (pathes[1] != null)
            foreach (string path in pathes[1])
            {
                enemyList.AddRange(await DataManager.DeserializeListJson<EnemyData>(path));
            }
        TotalProgress += enemyList.Count;

        if (pathes[2] != null)
            TotalProgress += pathes[2].Count;

        editingTowerSpritePath = editingPath + "Sprites/Tower/";
        foreach (var data in towerList)
        {
            AddData(data);
        }

        editingEnemySpritePath = editingPath + "Sprites/Enemy/";
        foreach (var data in enemyList)
        {
            AddData(data);
        }

        if (pathes[2] != null)
            foreach (var path in pathes[2])
            {
                AddData(path);
            }
    }

    public static async void AddData(TowerData data)
    {
        int id = data.id;

        Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(id, data);
        Tower tower = new Tower(data, anim);

        Sprite[] effect = await MakeObjects(id, data, "EFFECT");
        Sprite[] proj = await MakeObjects(id, data, "WEAPON");

        Sprite sprite = await DataManager.LoadSprite(editingTowerSpritePath + $"{id}/" + "IDLE.png", data.pivot, data.pixelperunit);
        if (sprite == null) sprite = await DataManager.LoadSprite(editingTowerSpritePath + $"{id}/" + "IDLE0.png", data.pivot, data.pixelperunit);

        if (towerKeys.Contains(id) == false)
        {
            towerKeys.Add(id);
            editingTowerData.Add(id, tower);
            editingTowerEffects.Add(id, effect);
            editingTowerProjs.Add(id, proj);
            editingTowerSprites.Add(id, sprite);
        }
        else
        {
            editingTowerData[id] = tower;
            editingTowerEffects[id] = effect;
            editingTowerProjs[id] = proj;
            editingTowerSprites[id] = sprite;
        }

        CurProgress++;
    }

    private static async Task<Dictionary<AnimationType, Sprite[]>> MakeAnimation(int id, TowerData data)
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
                Sprite sprite = await DataManager.LoadSprite(editingTowerSpritePath + $"{id}/" + filename, data.pivot, data.pixelperunit);
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

    private static async Task<Sprite[]> MakeObjects(int id, TowerData data, string type)
    {
        // 투사체의 이름은 WEAPON*
        // 이펙트의 이름은 EFFECT*
        Sprite[] s = null;

        List<Sprite> sprites = new List<Sprite>();
        while (true)
        {
            // IDLE0.png 와 같은 방식
            string filename = type + sprites.Count + ".png";
            Sprite sprite = await DataManager.LoadSprite(editingTowerSpritePath + $"{id}/" + filename, data.pivot, data.pixelperunit);
            if (sprite == null) break; // 이미지가 없다면 패스
            sprites.Add(sprite);
        }

        if (sprites.Count > 0)
        {
            s = new Sprite[sprites.Count];
            sprites.CopyTo(s);
        }

        return s;
    }

    public static async void AddData(EnemyData data)
    {
        int id = data.id;

        Dictionary<AnimationType, Sprite[]> anim = await MakeAnimation(id, data);

        Enemy enemy = new Enemy(data, anim);
        Sprite sprite = await DataManager.LoadSprite(editingEnemySpritePath + $"{id}/" + "IDLE.png", data.pivot, data.pixelperunit);
        if (sprite == null) sprite = await DataManager.LoadSprite(editingEnemySpritePath + $"{id}/" + "IDLE0.png", data.pivot, data.pixelperunit);

        if (enemyKeys.Contains(id) == false)
        {
            enemyKeys.Add(id);
            editingEnemyData.Add(id, enemy);
            editingEnemySprites.Add(id, sprite);
        }
        else
        {
            editingEnemyData[id] = enemy;
            editingEnemySprites[id] = sprite;
        }

        CurProgress++;
    }

    public static void RemoveData(int id)
    {
        if (editingTowerData.ContainsKey(id))
        {
            towerKeys.Remove(id);
            editingTowerData.Remove(id);
            editingTowerEffects.Remove(id);
            editingTowerProjs.Remove(id);
            editingTowerSprites.Remove(id);
            datas[editingDataIndex].dataAmount[0]--;
        }
        else if (editingEnemyData.ContainsKey(id))
        {
            int index = enemyKeys.FindIndex(i => i == id);

            enemyKeys.Remove(id);
            editingEnemyData.Remove(id);
            editingEnemySprites.Remove(id);
            datas[editingDataIndex].dataAmount[1]--;
        }
    }

    private static async Task<Dictionary<AnimationType, Sprite[]>> MakeAnimation(int id, EnemyData data)
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
                Sprite sprite = await DataManager.LoadSprite(editingEnemySpritePath + $"{id}/" + filename, data.pivot, data.pixelperunit);
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


    private static async void AddData(string path)
    {
        string mapName = DataManager.FileNameTriming(path);

        TilemapInfoJson data = await DataManager.DeserializeJson<TilemapInfoJson>(path);
        if (data == null) return;

        TilemapInfo info = new TilemapInfo(data);
        editingMapNames.Add(mapName);
        editingMapData.Add(mapName, new Map(mapName, info));

        CurProgress++;
    }

    public static void SaveMap(string mapName, TilemapInfo info)
    {
        TilemapInfoJson data = new TilemapInfoJson(info);
        DataManager.SerializeJson(editingMapPath, mapName, data);
        Map map = new Map(mapName, info);

        if (editingMapData.ContainsKey(mapName) == false)
        {
            string path = CustomDataManager.path + editingDataName + "/Map/" + mapName + ".json";
            editingMapData.Add(mapName, map);
            datas[editingDataIndex].dataAmount[2]++;
            datas[editingDataIndex].pathes[2].Add(path);
            datas[editingDataIndex].pathes[2].Sort();
        }
        else
        {
            editingMapData[mapName] = map;
        }
    }
}

public class CustomData
{
    public string name;
    // 0: Tower, 1: Enemy, 2: Map
    public List<string>[] pathes = new List<string>[3];
    public int[] dataAmount = new int[3] { 0, 0, 0 };
}