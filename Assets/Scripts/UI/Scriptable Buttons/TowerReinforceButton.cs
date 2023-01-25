using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerReinforceButton : ScriptableButton
{
    [SerializeField] private TowerUpgradeItem upgradeItem;

    protected override void ClickEvent()
    {
        UIController.Instance.ReinforceTower(upgradeItem.Index, upgradeItem.Type);
    }
}
