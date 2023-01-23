using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildTowerButton : ScriptableButton
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI costText;
    private Image gradeBackGroundImage;
    private int id;

    public void Init()
    {
        gradeBackGroundImage = GetComponent<Image>();
    }

    public void SetItem(Tower tower)
    {
        if (tower == null)
        {
            id = -1;
            image.sprite = null;
            costText.text = "";
            return;
        }
        id = tower.id;
        image.sprite = SpriteManager.GetSprite(id);
        costText.text = "$ " + PlayerController.Cost(tower.cost);
    }

    protected override void ClickEvent()
    {
        if (id != -1) UIController.Instance.BuildTower(id);
    }
}
