using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DescriptionIcon : MonoBehaviour
{
    private int iconId;
    
    private Image image;
    private bool isOn;

    private Vector3 mousePos;
    private bool contains;

    public void SetIcon(int id)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            isOn = false;
        }

        iconId = id;
        Sprite sprite = SpriteManager.GetSprite(id);
        if (sprite != null)
        {
            image.sprite = SpriteManager.GetSprite(id);
            image.color = Color.white;
        }
        else image.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        // Off되어있을 때에는 마우스의 위치가 Description Icon과 겹치는지 체크.
        // On되어있을때는 마우스가 Icon에서 나갔는지를 체크
        mousePos = Input.mousePosition;
        contains = UIController.PointOverUI(gameObject);
        
        if (isOn && !contains)
        {
            UIController.Instance.SetDescription(mousePos, null);
            isOn = false;
        }
    }

    private void LateUpdate()
    {
        if (contains)
        {
            if (isOn == false)
            {
                Language lang = Translator.GetLanguage(iconId);
                if (lang != null)
                    UIController.Instance.SetDescription(mousePos, lang.name);
                isOn = true;
            }
            else
            {
                UIController.Instance.MoveDescription(mousePos);
            }
        }
    }
}
