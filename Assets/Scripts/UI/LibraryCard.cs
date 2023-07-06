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
    protected Image gradeImage;
    [SerializeField] protected TextMeshProUGUI costText;
    [SerializeField] protected GameObject[] units;

    public Enemy EnemyData { get { return enemy; } }
    protected Enemy enemy;

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
        if (iconSlot != null)
            iconSlot.sizeDelta = new Vector2(iconImage.sprite.texture.width, iconImage.sprite.texture.height) / 24 * 100;

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

        Color c = data.GradeColor;

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;

        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        if (enemy == null) costText.text = data.cost.ToString();
        else
        {
            mainStats[0].SetData(enemy.hp);
            mainStats[1].SetData(enemy.speed);
            mainStats[2].SetData(enemy.exp);

            costText.text = enemy.money.ToString();
        }
    }

    public override void UpdateLanguage()
    {
        if (enemy != null) nameText.text = enemy.name;
        else if (data != null) nameText.text = data.name;
    }
}