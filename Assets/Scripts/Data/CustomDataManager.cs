using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using EnumData;
using System.Threading.Tasks;

public static class CustomDataManager
{
    public static string path { get; private set; } = "/Data/Custom/";
    public static List<CustomData> Datas { get { return datas; } }
    private static List<CustomData> datas;

    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static List<Tower> EditingTowerData { get { return editingTowerData; } }
    private static List<Tower> editingTowerData;
    public static List<Sprite> EditingTowerSprites { get { return editingTowerSprites; } }
    private static List<Sprite> editingTowerSprites;
    public static List<Sprite[]> EditingTowerEffects { get { return editingTowerEffects; } }
    private static List<Sprite[]> editingTowerEffects;
    public static List<Sprite[]> EditingTowerProjs { get { return editingTowerProjs; } }
    private static List<Sprite[]> editingTowerProjs;

    public static List<Sprite> EditingEnemySprites { get { return editingEnemySprites; } }
    private static List<Sprite> editingEnemySprites;
    public static List<Enemy> EditingEnemyData { get { return editingEnemyData; } }
    private static List<Enemy> editingEnemyData;

    public static List<string> EditingMapNames { get { return editingMapNames; } }
    private static List<string> editingMapNames;
    public static List<Map> EditingMapData { get { return editingMapData; } }
    private static List<Map> editingMapData;

    public static void GetTotal()
    {
        TotalProgress = 1;
    }

    public static async Task Init()
    {
        datas = await DataManager.LoadCustomDataList(path);
        CurProgress += 1;

        editingTowerData = new List<Tower>();
        editingTowerSprites = new List<Sprite>();
        editingTowerEffects = new List<Sprite[]>();
        editingTowerProjs = new List<Sprite[]>();
        editingEnemyData = new List<Enemy>();
        editingEnemySprites = new List<Sprite>();
        editingMapNames = new List<string>();
        editingMapData = new List<Map>();

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD CUSTOM DATA LIST");
#endif
    }

    public static async void LoadCustomData(List<string>[] pathes)
    {
        CurProgress = 0;
        TotalProgress = 9999;

        editingTowerData.Clear();
        editingTowerSprites.Clear();
        editingTowerEffects.Clear();
        editingTowerProjs.Clear();
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

        foreach (var data in towerList)
        {
            AddData(data);
        }

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

    private static async void AddData(TowerData data)
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

        Dictionary<AnimationType, Sprite[]> anim = await TowerManager.MakeAnimation(data);
        Tower tower = new Tower(data, anim);
        editingTowerData.Add(tower);

        Sprite[] effect = await TowerManager.MakeObjects(data, "EFFECT");
        editingTowerEffects.Add(effect);

        Sprite[] proj = await TowerManager.MakeObjects(data, "WEAPON");
        editingTowerProjs.Add(proj);

        Sprite sprite = await DataManager.LoadSprite(data.imgsrc + "IDLE.png", data.pivot, data.pixelperunit);
        if (sprite == null) sprite = await DataManager.LoadSprite(data.imgsrc + "IDLE0.png", data.pivot, data.pixelperunit);
        editingTowerSprites.Add(sprite);

        CurProgress++;
    }

    private static async void AddData(EnemyData data)
    {
        int originId = data.id;

        // id는 AEEGNNN으로 되어있음.
        int element, grade;

        element = originId % 1000000; // A 제거
        element = element / 10000; // GNNN 제거

        grade = originId % 10000; // AEE 제거
        grade = grade / 1000; // NNN 제거

        Dictionary<AnimationType, Sprite[]> anim = await EnemyManager.MakeAnimation(data);

        Enemy enemy = new Enemy(data, anim);
        editingEnemyData.Add(enemy);
        Sprite sprite = await DataManager.LoadSprite(data.imgsrc + "IDLE.png", data.pivot, data.pixelperunit);
        if (sprite == null) sprite = await DataManager.LoadSprite(data.imgsrc + "IDLE0.png", data.pivot, data.pixelperunit);
        editingEnemySprites.Add(sprite);

        CurProgress++;
    }

    private static async void AddData(string path)
    {
        string mapName = DataManager.FileNameTriming(path);

        TilemapInfoJson data = await DataManager.DeserializeJson<TilemapInfoJson>(path);
        if (data == null) return;

        TilemapInfo info = new TilemapInfo(data);
        editingMapNames.Add(mapName);
        editingMapData.Add(new Map(mapName, info));

        CurProgress++;
    }
}

public class CustomData
{
    public string name;
    // 0: Tower, 1: Enemy, 2: Map
    public List<string>[] pathes = new List<string>[3];
    public int[] dataAmount = new int[3] { 0, 0, 0 };
}