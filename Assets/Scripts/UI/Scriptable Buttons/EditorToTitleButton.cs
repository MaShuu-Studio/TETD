using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToTitleButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.EndEditor();
    }
}
