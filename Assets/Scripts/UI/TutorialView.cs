using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class TutorialView : MonoBehaviour
{
    [SerializeField] private bool nextTutorial;

    [SerializeField] private Image mask;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI text;

    public bool isShowing { get; private set; }

    public void SetView(TutorialControler tutorialController, string str)
    {
        isShowing = false;

        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Button button = GetComponent<Button>();
        if (nextTutorial) button.onClick.AddListener(() => tutorialController.NextTutorial());
        else button.onClick.AddListener(() => tutorialController.CloseTutorial());

        background.rectTransform.pivot = mask.rectTransform.pivot;
        background.rectTransform.anchorMin = mask.rectTransform.anchorMin;
        background.rectTransform.anchorMax = mask.rectTransform.anchorMax;
        background.rectTransform.anchoredPosition = mask.rectTransform.anchoredPosition * -1;

        background.color = new Color(0, 0, 0, 0.5f);

        gameObject.SetActive(false);
        //text.text = str;
    }

    public void ShowTutorial()
    {
        isShowing = true;
        gameObject.SetActive(true);
    }
}
