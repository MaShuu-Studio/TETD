using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMapButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.EditMap();
    }
}
