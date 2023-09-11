using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitEditorUnitIcon : ScriptableButton
{
    [SerializeField] private Image icon;
    private int id;

    public void Init(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    protected override void ClickEvent()
    {
        Tower tower = TowerManager.GetTower(id);
        if (tower != null)
        {
            UIController.Instance.UpdatePoster(tower);
        }
        else
        {
            Enemy enemy = EnemyManager.GetEnemy(id);
            UIController.Instance.UpdatePoster(enemy);
        }
    }
}
