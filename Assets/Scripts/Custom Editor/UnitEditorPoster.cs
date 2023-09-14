using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

public class UnitEditorPoster : LibraryCard
{
    public void ChangePosterType(bool isTower)
    {
        if (isTower)
        {
            for (int i = 0; i < mainStats.Length; i++)
            {
                TowerStatType type = (TowerStatType)i;
                mainStats[i].Init(type);
            }
        }
        else
        {
            for (int i = 0; i < mainStats.Length; i++)
            {
                EnemyStatType type = (EnemyStatType)i;
                mainStats[i].Init(type);
            }
        }
    }

    public void UpdatePoster(bool isTower, string name, int element, int grade, string cost)
    {
        nameText.text = name;
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, element);

        Color c;
        
        if (isTower) c = Tower.Color((Grade)grade);
        else c = Enemy.Color((EnemyGrade)grade);

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;

        costText.text = cost;
    }

    public void UpdateStat(float[] stats)
    {
        for (int i = 0; i < mainStats.Length; i++)
        {
            mainStats[i].SetData(stats[i]);
        }
    }

    public void UpdateAbility(Dictionary<AbilityType, float> abils)
    {
        int abilIndex = 0;
        foreach (var type in abils.Keys)
        {
            abilities[abilIndex].gameObject.SetActive(true);
            abilities[abilIndex].Init(type);
            abilities[abilIndex++].SetData(abils[type]);
        }

        for (; abilIndex < abilities.Length; abilIndex++)
        {
            abilities[abilIndex].gameObject.SetActive(false);
        }
    }

    public void UpdatePortrait(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
}
