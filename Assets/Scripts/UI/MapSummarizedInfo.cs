using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

public class MapSummarizedInfo : MonoBehaviour
{
    [SerializeField] private Image elementImage;
    [SerializeField] private List<GameObject> updowns;

    public enum UpDown { UP = 0, MID, DOWN }

    public void SetInfo(Element element, UpDown upDown)
    {
        for (int i = 0; i < updowns.Count; i++)
            updowns[i].SetActive(false);

        updowns[(int)upDown].SetActive(true);
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)element);
    }
}
