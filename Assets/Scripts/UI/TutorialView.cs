using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class TutorialView : MonoBehaviour
{
    [SerializeField] private bool nextTutorial;

    [SerializeField] private Image mask;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;

    public bool isProgress { get; private set; }

    public void SetView(TutorialController tutorialController)
    {
        isProgress = true;

        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Button button = GetComponent<Button>();
        if (nextTutorial) button.onClick.AddListener(() => tutorialController.NextTutorial());
        else button.onClick.AddListener(() => tutorialController.CloseTutorial());

        background.rectTransform.pivot = mask.rectTransform.pivot;
        background.rectTransform.anchorMin = mask.rectTransform.anchorMin;
        background.rectTransform.anchorMax = mask.rectTransform.anchorMax;
        background.rectTransform.anchoredPosition = mask.rectTransform.anchoredPosition * -1;

        gameObject.SetActive(false);
    }

    public void SetProgress(bool b)
    {
        isProgress = b;
    }

    public void SetText(string str)
    {
        text.text = str;
    }

    public void ShowTutorial()
    {
        isProgress = false;
        gameObject.SetActive(true);
    }
}
