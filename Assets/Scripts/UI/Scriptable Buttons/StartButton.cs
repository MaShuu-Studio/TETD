using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.StartGame();
    }
}
