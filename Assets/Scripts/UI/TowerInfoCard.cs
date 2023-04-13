using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfoCard : TowerInfo
{
    [Space]
    [SerializeField] private Image frameImage;
    [SerializeField] private Image gradeImage;
    [SerializeField] protected TextMeshProUGUI costText;

    public override void SetData(Tower data)
    {
        base.SetData(data);

        // 간단하게 등급 구분을 먼저 색으로만
        Color c = new Color(1, 1, 1);
        Color f = new Color(0.5f, 0.5f, 0.5f);
        switch ((Grade)data.grade)
        {
            case Grade.RARE:
                c = new Color(0.5f, 0.8f, 1);
                f = new Color(0.25f, 0.4f, 1);
                break;
            case Grade.HEROIC:
                c = new Color(0.7f, 0.4f, 1);
                f = new Color(0.35f, 0.2f, 1);
                break;
            case Grade.LEGENDARY:
                c = new Color(1, 0.4f, 0.2f);
                f = new Color(1, 0.1f, 0.05f);
                break;
        }

        frameImage.color = f;
        gradeImage.color = c;
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        costText.text = data.cost.ToString();
    }
}