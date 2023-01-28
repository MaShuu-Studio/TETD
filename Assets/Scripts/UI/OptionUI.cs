using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;

    private void Start()
    {
        bgmSlider.value = 10;
        sfxSlider.value = 10;
        SetSound();
    }

    public void SetSound()
    {
        bgmText.text = bgmSlider.value.ToString();
        sfxText.text = sfxSlider.value.ToString();

        SoundController.Instance.SetVolume(bgmSlider.value, sfxSlider.value);
    }
}
