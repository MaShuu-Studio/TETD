using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitEditorUnitIcon : ScriptableButton
{
    [SerializeField] private Image icon;
    private Tower tower;
    private Enemy enemy;

    public void Init(Tower tower, Sprite sprite)
    {
        this.tower = tower;
        enemy = null;
        icon.sprite = sprite;
    }
    public void Init(Enemy enemy, Sprite sprite)
    {
        this.tower = null;
        this.enemy = enemy;
        icon.sprite = sprite;
    }

    protected override void ClickEvent()
    {
        if (tower != null)
        {
            UIController.Instance.UpdatePoster(tower);
        }
        else
        {
            UIController.Instance.UpdatePoster(enemy);
        }
    }
}
