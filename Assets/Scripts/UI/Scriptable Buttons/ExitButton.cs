using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        Application.Quit();
    }
}
