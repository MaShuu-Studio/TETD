using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Data;

public static class Translator
{
    private static string path = Application.streamingAssetsPath + "/Data/";

    private static Dictionary<int, Language> languages;

    public static async Task Init()
    {
        languages = new Dictionary<int, Language>();
        List<LanguageData> list = await DataManager.DeserializeListJson<LanguageData>(path, "TowerLang");

        foreach (var data in list)
        {
            Language lang = new Language(data);
            languages.Add(lang.id, lang);
        }
        list = await DataManager.DeserializeListJson<LanguageData>(path, "EnemyLang");

        foreach (var data in list)
        {
            Language lang = new Language(data);
            languages.Add(lang.id, lang);
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
