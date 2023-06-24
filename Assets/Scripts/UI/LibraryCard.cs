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

        // �����ϰ� ��� ������ ���� �����θ�
        Color c = data.GradeColor;

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;
    }

    // Ÿ�� �������� ���¸� Enemy�� �����ؼ� Ȱ��
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

        // UI Icon�鵵 id�� ���̰� SpriteManager���� ���� ����
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