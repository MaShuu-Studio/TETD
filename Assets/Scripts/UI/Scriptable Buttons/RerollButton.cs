using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        UIController.Instance.RerollAll();
    }
}
