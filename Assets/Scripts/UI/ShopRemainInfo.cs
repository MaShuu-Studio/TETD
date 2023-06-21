using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopRemainInfo : MonoBehaviour
{
    [SerializeField] private Image elementImage;
    [SerializeField] private TextMeshProUGUI remainText;

    public void Init(int elementNumber)
    {
        Sprite sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, elementNumber);
        elementImage.sprite = sprite;
    }

    public void SetData(string amount)
    {
        remainText.text = amount;
    }
}
