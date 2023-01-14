using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoItem : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    [SerializeField] private Image gradeImage;
    [SerializeField] private Image elementImage;
    [SerializeField] private List<Sprite> elementSprites;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI costText;

    private Tower data;
    public Tower Data { get { return data; } }

    public void SetData(Tower data)
    {
        this.data = data;

        nameText.text = data.name.Substring(6);
        iconImage.sprite = SpriteManager.GetSprite(data.id);

        // 간단하게 등급 구분을 먼저 색으로만
        Color c = new Color(1, 1, 1);
        Color f = new Color(0.5f, 0.5f, 0.5f);
        switch ((EnumData.Grade)data.grade)
        {
            case EnumData.Grade.RARE:
                c = new Color(0.5f, 0.8f, 1);
                f = new Color(0.25f, 0.4f, 1);
                break;
            case EnumData.Grade.HEROIC:
                c = new Color(0.7f, 0.4f, 1);
                f = new Color(0.35f, 0.2f, 1);
                break;
            case EnumData.Grade.LEGENDARY:
                c = new Color(1, 0.4f, 0.2f);
                f = new Color(1, 0.1f, 0.05f);
                break;
        }
        frameImage.color = f;
        gradeImage.color = c;

        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = elementSprites[data.element];

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        damageText.text = data.dmg.ToString();
        attackSpeedText.text = data.attackspeed.ToString();
        rangeText.text = data.range.ToString();
        costText.text = data.cost.ToString();
    }

    private void AddSkill()
    {

    }
}
