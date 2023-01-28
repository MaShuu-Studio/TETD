using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.Title();
    }
}
