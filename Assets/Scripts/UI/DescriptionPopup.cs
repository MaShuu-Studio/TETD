using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descText;
    private const int maxWidth = 150;
    private RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    public void SetDescription(Vector3 pos, string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            gameObject.SetActive(false);
            return;
        }

        /* wrapping을 풀어놓은 뒤 text content의 사이즈를 체크.
         * 최대 사이즈를 제한하여 그에 따라 height 추가 조정
         * 이 후 다시 wrapping하여 박스에 알맞게 들어가도록 함.
         */

        descText.enableWordWrapping = false;
        descText.text = str;

        if (descText.preferredWidth > maxWidth)
        {
            float height = Mathf.CeilToInt(descText.preferredWidth / maxWidth) * descText.preferredHeight;
            rect.sizeDelta = new Vector2(maxWidth, height);
            descText.enableWordWrapping = true;
        }
        else
        {
            rect.sizeDelta = new Vector2(descText.preferredWidth, descText.preferredHeight);
        }

        MoveDescription(pos);
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public void MoveDescription(Vector3 pos)
    {
        /* 사이즈 조정이 끝난 후 Description이 밖으로 나갈 것 같다면 위치조정.
         * 우측으로 나간다면 pos의 x를 rect만큼 뺌.
         * 아래로 나간다면 pos의 y를 rect만큼 더함.
         */

        Vector2 size = new Vector2(
                pos.x + rect.sizeDelta.x,
                pos.y - rect.sizeDelta.y);

        if (size.x > Screen.width) pos.x -= rect.sizeDelta.x;
        if (size.y > Screen.height * -1) pos.y += rect.sizeDelta.y;

        // anchor 조정 후 위치 변경.
        transform.position = pos;
    }
}
