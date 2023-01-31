using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TMP_Dropdown languages;

    public async void Init()
    {
        bgmSlider.value = 10;
        sfxSlider.value = 10;
        SetSound();

        while (EnumData.EnumArray.LanguageTypeStrings == null) await Task.Yield();
        languages.options.Clear();
        languages.AddOptions(EnumData.EnumArray.LanguageTypeStrings.Values.ToList());
        languages.onValueChanged.AddListener(i => GameController.Instance.SetLanguage(i));
    }

    public void SetSound()
    {
        bgmText.text = bgmSlider.value.ToString();
        sfxText.text = sfxSlider.value.ToString();

        SoundController.Instance.SetVolume(bgmSlider.value, sfxSlider.value);
    }
}
