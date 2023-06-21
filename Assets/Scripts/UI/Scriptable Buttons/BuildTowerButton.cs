using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class BuildTowerButton : ScriptableButton
{
    [SerializeField] private Image image;
    [SerializeField] private Image elementImage;
    [SerializeField] private TextMeshProUGUI costText;
    private Image gradeImage;
    private int id;

    protected override void Awake()
    {
        base.Awake();
        gradeImage = GetComponent<Image>();
    }

    public void SetItem(Tower tower)
    {
        if (tower == null)
        {
            id = -1;
            image.sprite = null;
            elementImage.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            costText.text = "";
            gradeImage.color = Color.white;
            return;
        }

        id = tower.id;
        image.sprite = SpriteManager.GetSprite(id);
        image.gameObject.SetActive(true);
        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT,(int)tower.element);
        elementImage.gameObject.SetActive(true);
        costText.text = "$ " + PlayerController.Cost(tower.cost);

        gradeImage.color = tower.GradeColor;
    }

    protected override void ClickEvent()
    {
        if (id != -1) UIController.Instance.BuildTower(id);
    }
}
