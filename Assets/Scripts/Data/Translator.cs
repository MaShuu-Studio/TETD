using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Data;
using EnumData;

public static class Translator
{
    public static string path { get; private set; } = "/Data/Language/";

    private static Dictionary<int, Language>[] languages;
    public static List<string> Langs { get { return langs; } }
    private static List<string> langs;

    public static List<int> Keys { get { return keys; } }
    private static List<int> keys;
    public static LanguageType CurrentLanguage { get { return currentLanguage; } }
    private static LanguageType currentLanguage;
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static async Task GetTotal()
    {
        TotalProgress = 0;

        langs = DataManager.GetFileNames(path);

        for (int i = 0; i < langs.Count; i++)
        {
            langs[i] = langs[i].Split(".")[0];
            List<LanguageData> l = await DataManager.DeserializeListJson<LanguageData>(path, langs[i]);
            TotalProgress += l.Count;
        }
    }
    public static async Task Init()
    {
        languages = new Dictionary<int, Language>[langs.Count];
        for (int i = 0; i < langs.Count; i++)
        {
            CurProgress++;

            List<LanguageData> l = await DataManager.DeserializeListJson<LanguageData>(path, langs[i]);
            languages[i] = new Dictionary<int, Language>();

            foreach (var data in l)
            {
                Language lang = new Language(data);
                languages[i].Add(lang.id, lang);
            }
        }
        keys = languages[0].Keys.ToList();
#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD LANGUAGES {CurProgress}");
#endif
    }

    public static void SetLanguage(int lang)
    {
        currentLanguage = (LanguageType)lang;
        TowerManager.UpdateLanguage(currentLanguage);
        EnemyManager.UpdateLanguage(currentLanguage);

        UIController.Instance.UpdateLanguage();

        // 인게임이 아니라면 TowerController가 존재하지 않음.
        if (TowerController.Instance != null)
            TowerController.Instance.UpdateLanguage(currentLanguage);
    }

    public static Language GetLanguage(int id)
    {
        int cur = (int)currentLanguage;
        if (languages[cur].ContainsKey(id)) return languages[cur][id];
        return null;
    }
    public static Language[] GetLanguages(int id)
    {
        if (languages[0].ContainsKey(id))
        {
            Language[] arr = new Language[langs.Count];
            for (int i = 0; i < langs.Count; i++)
            {
                arr[i] = languages[i][id];
            }
            return arr;
        }
        return null;
    }

    public static void AddData(int id, Language[] langs)
    {
        if (languages[0].ContainsKey(id))
        {
            for (int i = 0; i < langs.Length; i++)
            {
                languages[i][id] = langs[i];
            }
        }
        else
        {
            for (int i = 0; i < langs.Length; i++)
            {
                languages[i].Add(id, langs[i]);
            }
        }
    }
}
