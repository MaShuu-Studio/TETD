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

    public static void GetTotal()
    {
        TotalProgress = 1;
    }

    public static async Task Init()
    {
        datas = await DataManager.LoadCustomDataList(path);
        CurProgress += 1;

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD CUSTOM DATA LIST");
#endif
    }
}

public class CustomData
{
    public string name;
    // 0: Tower, 1: Enemy, 2: Map
    public List<string>[] pathes = new List<string>[3];
    public int[] dataAmount = new int[3] { 0, 0, 0 };
}