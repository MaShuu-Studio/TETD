using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditUnitButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.EditUnit();
    }
}
