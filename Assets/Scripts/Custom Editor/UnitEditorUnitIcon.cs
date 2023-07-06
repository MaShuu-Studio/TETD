using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitEditorUnitIcon : ScriptableButton
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI idText;
    private int id;

    public void Init(int id)
    {
        icon.sprite = SpriteManager.GetSprite(id);
        idText.text = id.ToString();
        this.id = id;
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
