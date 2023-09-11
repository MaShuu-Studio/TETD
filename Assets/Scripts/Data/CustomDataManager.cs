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

        // CustomData�� index������ element, grade �� 1000���� ��Ƽ� Ȱ��. 0~999
        // �̸� �Ѵ´ٸ� ���̻� �߰��� �� ������ ��. ���� �ڵ� ���� �ʿ�.

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

        // id�� AEEGNNN���� �Ǿ�����.
        int element, grade;

        element = originId % 1000000; // A ����
        element = element / 10000; // GNNN ����

        grade = originId % 10000; // AEE ����
        grade = grade / 1000; // NNN ����

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