using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTowerButton : ScriptableButton
{
    [SerializeField] private TowerInfoItem item;
    protected override void ClickEvent()
    {
        PlayerController.Instance.AddTower(item.Data);
        UIController.Instance.UpdateTowerList();
        UIController.Instance.Reroll(item);
    }
}
