using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        PlayerController.Instance.LevelUp();
    }
}
