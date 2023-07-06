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
    public void UpdatePoster(bool isTower, string name, int element, int grade)
    {
        nameText.text = name;
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, element);

        Color c;
        
        if (isTower) c = Tower.Color((Grade)grade);
        else c = Enemy.Color((EnemyGrade)grade);

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;
    }

    public void UpdateStat(float[] stats)
    {
        for (int i = 0; i < mainStats.Length; i++)
        {
            mainStats[i].SetData(stats[i]);
        }
    }

    public void UpdateAbility(Dictionary<TowerStatType, float> statAbils, Dictionary<BuffType, float> buffs, Dictionary<DebuffType, float> debuffs)
    {
        int abilIndex = 0;
        foreach (var type in statAbils.Keys)
        {
            abilities[abilIndex].gameObject.SetActive(true);
            abilities[abilIndex].Init(type);
            abilities[abilIndex++].SetData(statAbils[type]);
        }

        foreach (var type in buffs.Keys)
        {
            abilities[abilIndex].gameObject.SetActive(true);
            abilities[abilIndex].Init(type);
            abilities[abilIndex++].SetData(buffs[type]);
        }

        foreach (var type in debuffs.Keys)
        {
            abilities[abilIndex].gameObject.SetActive(true);
            abilities[abilIndex].Init(type);
            abilities[abilIndex++].SetData(debuffs[type]);
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
