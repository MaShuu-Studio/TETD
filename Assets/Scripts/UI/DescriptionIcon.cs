using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionIcon : MonoBehaviour
{
    // ���� UI�� �����ͷ� �����ϰ� �Ǹ� ���� �κ�
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
            // Icon�� ��ġ�� Rect�� ��ȯ
            RectTransform rectTransform = (RectTransform)transform;
            Vector2 rectPos = new Vector2(
                rectTransform.position.x + rectTransform.rect.position.x,
                rectTransform.position.y + rectTransform.rect.position.y);

            rect = new Rect(rectPos, rectTransform.sizeDelta);
        }


        // Off�Ǿ����� ������ ���콺�� ��ġ�� Description Icon�� ��ġ���� üũ.
        // On�Ǿ��������� ���콺�� Icon���� ���������� üũ
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
