using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

[RequireComponent(typeof(Image))]
public class TowerInfoCard : TowerInfo
{
    [Space]
    private Image gradeImage;
    [SerializeField] protected TextMeshProUGUI costText;

    private void Awake()
    {
        gradeImage = GetComponent<Image>();
    }

    public override void SetData(Tower data)
    {
        base.SetData(data);

        // 간단하게 등급 구분을 먼저 색으로만
        Color c = data.GradeColor;

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        costText.text = data.cost.ToString();
    }
}