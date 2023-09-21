using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControler : MonoBehaviour
{
    public static bool isTutorial;
    private int showingIndex;
    [SerializeField] private List<TutorialView> tutorialViews;

    public void Init()
    {
        isTutorial = true;
        foreach (var view in tutorialViews)
        {
            view.SetView(this, "");
        }
    }

    public void ShowTutorial(int index)
    {
        if (tutorialViews[index].isShowing) return;

        GameController.Instance.Pause(true, true);

        tutorialViews[showingIndex].gameObject.SetActive(false);

        showingIndex = index;
        tutorialViews[index].ShowTutorial();
    }

    public void NextTutorial()
    {
        tutorialViews[showingIndex].gameObject.SetActive(false);
        showingIndex++;
        tutorialViews[showingIndex].ShowTutorial();
    }

    public void CloseTutorial()
    {
        tutorialViews[showingIndex].gameObject.SetActive(false);
        GameController.Instance.Pause(false, true);
    }

    public void UpdateLauguage()
    {

    }
}
