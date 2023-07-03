using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditMapButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.EditMap();
    }
}
