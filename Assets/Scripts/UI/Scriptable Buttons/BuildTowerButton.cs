using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class BuildTowerButton : ScriptableButton
{
    [SerializeField] private Image image;
    [SerializeField] private Image elementSlot;
    [SerializeField] private Image elementImage;
    [SerializeField] private TextMeshProUGUI costText;
    private Image gradeImage;
    private int id;
    private int index;

    public bool CanUse { get { return id != -1; } }

    protected override void Awake()
    {
        base.Awake();
        gradeImage = GetComponent<Image>();
        gradeImage.alphaHitTestMinimumThreshold = 0.1f;
    }

    public void SetIndex(int index)
    {
        this.index = index;
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
        ((RectTransform)image.transform).sizeDelta
            = new Vector2(image.sprite.texture.width, image.sprite.texture.height) / 24 * CameraController.ReferencePPU;
        image.gameObject.SetActive(true);
        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)tower.element);
        elementImage.gameObject.SetActive(true);
        costText.text = "$ " + PlayerController.Cost(tower.cost);

        elementSlot.color = tower.GradeColor;
        gradeImage.color = tower.GradeColor;
    }

    protected override void ClickEvent()
    {
        UIController.Instance.SelectTower(index);
    }

    public void ReadyToBuildTower()
    {
        if (id != -1)
            UIController.Instance.ReadyToBuildTower(id);
    }
}
