using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionIcon : MonoBehaviour
{
    // 추후 UI를 데이터로 관리하게 되면 빠질 부분
    [SerializeField] private string infoString;
    private Rect rect;
    private bool isOn;

    private bool isInitialized;

    private Vector3 mousePos;
    private bool contains;

    private void Start()
    {
        isOn = false;
        isInitialized = false;
    }

    void Update()
    {
        if (isInitialized == false)
        {
            // Icon의 위치를 Rect로 변환
            RectTransform rectTransform = (RectTransform)transform;
            Vector2 rectPos = new Vector2(
                rectTransform.position.x + rectTransform.rect.position.x,
                rectTransform.position.y + rectTransform.rect.position.y);

            rect = new Rect(rectPos, rectTransform.sizeDelta);
        }


        // Off되어있을 때에는 마우스의 위치가 Description Icon과 겹치는지 체크.
        // On되어있을때는 마우스가 Icon에서 나갔는지를 체크
        mousePos = Input.mousePosition;
        contains = rect.Contains(mousePos); 
        
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
                UIController.Instance.SetDescription(mousePos, infoString);
                isOn = true;
            }
            else
            {
                UIController.Instance.MoveDescription(mousePos);
            }
        }
    }
}
