using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        TowerController.Instance.SellTower();
    }
}
