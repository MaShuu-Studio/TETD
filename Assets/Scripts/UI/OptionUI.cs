using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using Data;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TMP_Dropdown languages;

    public async void Init()
    {
        Setting setting = DataManager.LoadSetting();
        int bgm = 10;
        int sfx = 10;
        int lang = 0;
        if (setting != null)
        {
            bgm = setting.Option("bgm");
            sfx = setting.Option("sfx");
            lang = setting.Option("language");
        }

        if (bgm == -1) bgm = 10;
        if (sfx == -1) sfx = 10;
        if (lang == -1) lang = 0;

        bgmSlider.value = bgm;
        sfxSlider.value = sfx;
        SetSound();

        while (EnumData.EnumArray.LanguageTypeStrings == null) await Task.Yield();
        languages.options.Clear();
        languages.AddOptions(EnumData.EnumArray.LanguageTypeStrings.Values.ToList());
        languages.onValueChanged.AddListener(i => GameController.Instance.SetLanguage(i));

        languages.value = lang;

        bgmSlider.onValueChanged.AddListener(v => SetOption());
        sfxSlider.onValueChanged.AddListener(v => SetOption());
        languages.onValueChanged.AddListener(v => SetOption());
    }

    public void SetSound()
    {
        bgmText.text = bgmSlider.value.ToString();
        sfxText.text = sfxSlider.value.ToString();

        SoundController.Instance.SetVolume(bgmSlider.value, sfxSlider.value);
    }

    public void SetOption()
    {
        Setting setting = new Setting();

        setting.AddOption("bgm", (int)bgmSlider.value);
        setting.AddOption("sfx", (int)sfxSlider.value);
        setting.AddOption("language", languages.value);

        DataManager.SaveSetting(setting);
    }
}

public class Setting
{
    private Dictionary<string, int> options;

    public int Option(string key)
    {
        key = key.ToLower();
        if (options.ContainsKey(key)) return options[key];
        return -1;
    }

    public Setting()
    {
        options = new Dictionary<string, int>();
    }

    public void AddOption(string key, int value)
    {
        key = key.ToLower();
        options.Add(key, value);
    }

    public new string ToString()
    {
        string str = $"[{typeof(Setting).Name}]";
        foreach (var key in options.Keys)
        {
            str += $"\n{key.ToLower()}={options[key]}";
        }

        return str;
    }

    public static bool TrimingValue(string line, out string key, out int value)
    {
        int equalIndex = line.IndexOf("=");

        if (equalIndex <= 0)
        {
            key = null;
            value = -1;
            return false;
        }
        key = line.Substring(0, equalIndex).ToLower();
        string v = line.Substring(equalIndex + 1);
        return int.TryParse(v, out value);
    }
}
