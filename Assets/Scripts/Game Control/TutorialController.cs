using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private int showingIndex;
    [SerializeField] private List<TutorialView> tutorialViews;

    public void Init()
    {
        showingIndex = -1;
        foreach (var view in tutorialViews)
        {
            view.SetView(this);
        }
    }

    public void SetProgress(bool b)
    {
        foreach (var view in tutorialViews)
        {
            view.SetProgress(b);
        }
    }

    public void ShowTutorial(int index)
    {
        if (tutorialViews[index].isProgress == false) return;

        GameController.Instance.TutorialPause(true);

        if (showingIndex != -1)
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
        GameController.Instance.TutorialPause(false);
        if (showingIndex == tutorialViews.Count - 1)
        {
            UIController.Instance.EndTutorial();
            showingIndex = -1;
        }
    }

    public void UpdateLauguage()
    {
        for (int i = 0; i < tutorialViews.Count; i++)
        {
            tutorialViews[i].SetText(Translator.GetLanguage((int)SpriteManager.ETCDataNumber.TUTORIALGAME + i).desc);
        }
    }
}
