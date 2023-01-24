using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] protected Image elementImage;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TowerStatItem[] mainStats;
    [SerializeField] protected TowerStatItem[] abilities;
    [SerializeField] protected TextMeshProUGUI nameText;

    protected Tower data;
    public Tower Data { get { return data; } }

    public virtual void SetData(Tower data)
    {
        this.data = data;

        nameText.text = data.name.Substring(6);
        iconImage.sprite = SpriteManager.GetSprite(data.id);

        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSprite($"Element{(int)data.element}");

        for (int i = 0; i < mainStats.Length; i++)
        {
            EnumData.TowerStatType type = (EnumData.TowerStatType)i;
            mainStats[i].Init(type);
        }
        int abilIndex = 0;
        if (data.StatTypes.Count > 3)
        {
            for (; abilIndex < data.StatTypes.Count - 3; abilIndex++)
            {
                EnumData.TowerStatType type = data.StatTypes[abilIndex + 3];
                abilities[abilIndex].gameObject.SetActive(true);
                abilities[abilIndex].Init(type);
            }
        }
        for (; abilIndex < abilities.Length; abilIndex++)
        {
            abilities[abilIndex].gameObject.SetActive(false);
        }
        UpdateInfo();
    }

    public virtual void UpdateInfo()
    {
        for (int i = 0; i < mainStats.Length; i++)
        {
            EnumData.TowerStatType type = (EnumData.TowerStatType)i;
            mainStats[i].SetData(data.Stat(type));
        }

        if (data.StatTypes.Count > 3)
        {
            for (int i = 0; i < data.StatTypes.Count - 3; i++)
            {
                EnumData.TowerStatType type = data.StatTypes[i + 3];
                abilities[i].SetData(data.Stat(type));
            }
        }
    }

    protected void AddSkill()
    {

    }
}
