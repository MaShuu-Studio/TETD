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
    [SerializeField] private Toggle tutorialToggle;

    [SerializeField] private TMP_Dropdown resolutions;
    [SerializeField] private TMP_Dropdown screenMode;
    [SerializeField] private Slider frameSlider;
    [SerializeField] private TextMeshProUGUI frameText;

    public async Task Init()
    {
        Setting setting = DataManager.LoadSetting();
        int bgm = -1;
        int sfx = -1;
        int lang = -1;
        int width = -1;
        int height = -1;
        int screenmode = -1;
        int frame = -1;
        bool tutorial = true;

        if (setting != null)
        {
            bgm = setting.Option("bgm");
            sfx = setting.Option("sfx");
            lang = setting.Option("language");
            width = setting.Option("width");
            height = setting.Option("height");
            screenmode = setting.Option("screenmode");
            frame = setting.Option("frame");
            tutorial = setting.Option("tutorial") == 1;
        }

        if (bgm == -1) bgm = 10;
        if (sfx == -1) sfx = 10;
        if (lang == -1) lang = 0;
        if (width == -1) width = Screen.currentResolution.width;
        if (height == -1) height = Screen.currentResolution.height;
        if (screenmode == -1) screenmode = (int)FullScreenMode.ExclusiveFullScreen;
        if (frame == -1) frame = Screen.currentResolution.refreshRate;

        bgmSlider.value = bgm;
        sfxSlider.value = sfx;
        SetSound();
        if (screenmode >= 2) screenmode = 2;
        screenMode.value = screenmode;
        ChangeScreenMode(screenmode);

        ChangeResolution(width, height);
        CameraController.Instance.ChangeResolution(width, height);

        while (Translator.Langs == null) await Task.Yield();
        languages.options.Clear();
        languages.AddOptions(Translator.Langs);
        languages.onValueChanged.AddListener(i => Translator.SetLanguage(i));

        // 언어 세팅. 그 전에 Tower와 Enemy가 로드되기를 대기함.
        while (TowerManager.Keys == null || EnemyManager.Keys == null) await Task.Yield();
        Translator.SetLanguage(lang);
        languages.value = lang;

        if (resols.Count > 0)
        {
            for (int i = 0; i < resols.Count; i++)
            {
                if (resols[i].width == width && resols[i].height == height)
                {
                    resolutions.value = i;
                }
            }
        }

        resolutions.onValueChanged.AddListener(v => ChangeResolution(v));
        screenMode.onValueChanged.AddListener(v => ChangeScreenMode(v));

        frameSlider.wholeNumbers = true;
        frameSlider.minValue = 30;
        frameSlider.maxValue = Screen.currentResolution.refreshRate;

        frameSlider.onValueChanged.AddListener(v => ChangeFrame((int)v));
        frameSlider.value = frame;

        tutorialToggle.isOn = !tutorial;
        tutorialToggle.onValueChanged.AddListener(b => UIController.Instance.SetTutorialProgress(b));
        tutorialToggle.isOn = tutorial;

        bgmSlider.onValueChanged.AddListener(v => SetOption());
        sfxSlider.onValueChanged.AddListener(v => SetOption());
        languages.onValueChanged.AddListener(v => SetOption());
        resolutions.onValueChanged.AddListener(v => SetOption());
        screenMode.onValueChanged.AddListener(v => SetOption());
        frameSlider.onValueChanged.AddListener(v => SetOption());
        tutorialToggle.onValueChanged.AddListener(v => SetOption());
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

        setting.AddOption("width", curResolution.x);
        setting.AddOption("height", curResolution.y);
        setting.AddOption("screenmode", (int)curScreenMode);

        setting.AddOption("frame", Application.targetFrameRate);
        setting.AddOption("tutorial", (tutorialToggle.isOn) ? 1 : 0);

        DataManager.SaveSetting(setting);
    }

    List<Resolution> resols = new List<Resolution>();
    FullScreenMode curScreenMode;
    Vector2Int curResolution;
    private void SetResolutionOptions(bool b)
    {
        resolutions.ClearOptions();
        resols.Clear();

        if (b)
        {
            List<string> list = new List<string>();
            foreach (var res in Screen.resolutions)
            {
                if (res.width * 9 != res.height * 16) continue;
                bool skip = false;
                foreach (var r in resols)
                {
                    if (res.width == r.width && res.height == r.height)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;
                resols.Add(res);
                string str = $"{res.width}x{res.height}";
                list.Add(str);
            }
            resolutions.AddOptions(list);
        }
    }

    private void ChangeResolution(int index)
    {
        Screen.SetResolution(resols[index].width, resols[index].height, curScreenMode);
        CameraController.Instance.ChangeResolution(resols[index].width, resols[index].height);
        curResolution = new Vector2Int(resols[index].width, resols[index].height);
    }

    private void ChangeResolution(int width, int height)
    {
        Screen.SetResolution(width, height, curScreenMode);
        CameraController.Instance.ChangeResolution(width, height);
        curResolution = new Vector2Int(width, height);
    }

    private void ChangeScreenMode(int mode)
    {
        if (mode >= 2) mode = 3;
        Screen.fullScreenMode = curScreenMode = (FullScreenMode)mode;
        SetResolutionOptions(mode != 1);

        // 전체창모드는 현재 디스플레이 설정에 맞게 해상도 자동세팅.
        if (mode == 1)
        {
            int width = Screen.mainWindowDisplayInfo.width;
            int height = Screen.mainWindowDisplayInfo.height;
            Resolution resol = new Resolution()
            {
                width = width,
                height = height
            };
            resols.Add(resol);
            resolutions.AddOptions(new List<string>() { $"{width}x{height}" });
            resolutions.value = 0;

            ChangeResolution(width, height);
        }
        else
        {
            for (int i = 0; i < resols.Count; i++)
            {
                if (resols[i].width == Screen.currentResolution.width
                    && resols[i].height == Screen.currentResolution.height)
                {
                    resolutions.value = i;
                    break;
                }
            }
        }
    }

    private void ChangeFrame(int frame)
    {
        Application.targetFrameRate = frame;
        frameText.text = frame.ToString();
    }

    public void EndTutorial()
    {
        tutorialToggle.isOn = false;
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
