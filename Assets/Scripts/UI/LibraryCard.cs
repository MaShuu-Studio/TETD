using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

[RequireComponent(typeof(Image))]
public class LibraryCard : TowerInfo
{
    [Space]
    private Image gradeImage;
    [SerializeField] protected TextMeshProUGUI costText;
    [SerializeField] private GameObject[] units;

    private Enemy enemy;

    private void Awake()
    {
        gradeImage = GetComponent<Image>();
    }

    public override void SetData(Tower data)
    {
        enemy = null;
        for(int i = 0; i < units.Length; i++)
        {
            units[i].SetActive(true);
        }
        base.SetData(data);

        // 간단하게 등급 구분을 먼저 색으로만
        Color c = data.GradeColor;

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;
    }

    // 타워 데이터의 형태를 Enemy로 변형해서 활용
    public void SetData(Enemy data)
    {
        for (int i = 0; i < units.Length; i++)
        {
            units[i].SetActive(false);
        }
        enemy = data;
        gradeImage.color = new Color(.8f, .8f, .8f);

        nameText.text = data.name;
        iconImage.sprite = SpriteManager.GetSprite(data.id);

        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)data.element);

        for (int i = 0; i < mainStats.Length; i++)
        {
            EnemyStatType type = (EnemyStatType)i;
            mainStats[i].Init(type);
        }
        
        int abilIndex = 0;
        for (; abilIndex < abilities.Length; abilIndex++)
        {
            abilities[abilIndex].gameObject.SetActive(false);
        }
        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        if (enemy == null) costText.text = data.cost.ToString();
        else costText.text = enemy.money.ToString();
    }
}