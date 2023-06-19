using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Data;

public static class Translator
{
    private static string path = "/Data/";

    private static Dictionary<int, Language> languages;
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }

    public static async Task GetTotal()
    {
        List<LanguageData> towers = await DataManager.DeserializeListJson<LanguageData>(path, "TowerLang");
        List<LanguageData> enemys = await DataManager.DeserializeListJson<LanguageData>(path, "EnemyLang");

        TotalProgress = towers.Count + enemys.Count;
    }
    public static async Task Init()
    {
        languages = new Dictionary<int, Language>();
        List<LanguageData> towers = await DataManager.DeserializeListJson<LanguageData>(path, "TowerLang");
        List<LanguageData> enemys = await DataManager.DeserializeListJson<LanguageData>(path, "EnemyLang");

        foreach (var data in towers)
        {
            Language lang = new Language(data);
            languages.Add(lang.id, lang);

            CurProgress++;
        }

        foreach (var data in enemys)
        {
            Language lang = new Language(data);
            languages.Add(lang.id, lang);

            CurProgress++;
        }

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD LANGUAGES {languages.Count}");
#endif
    }

    public static Language GetLanguage(int id)
    {
        if (languages.ContainsKey(id)) return languages[id];
        return null;
    }
}
