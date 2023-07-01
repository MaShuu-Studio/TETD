using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] protected Image elementImage;
    //[SerializeField] protected RectTransform iconSlot;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TowerStatItem[] mainStats;
    [SerializeField] protected TextMeshProUGUI attackAmount;
    [SerializeField] protected TowerStatItem[] abilities;
    [SerializeField] protected TextMeshProUGUI nameText;

    protected Tower data;
    public Tower Data { get { return data; } }

    public virtual void SetData(Tower data)
    {
        this.data = data;

        nameText.text = data.name;
        iconImage.sprite = SpriteManager.GetSprite(data.id);
        //iconSlot.sizeDelta = new Vector2(iconImage.sprite.texture.width, iconImage.sprite.texture.height) / 24 * 100;

        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT,(int)data.element);

        for (int i = 0; i < mainStats.Length; i++)
        {
            EnumData.TowerStatType type = (EnumData.TowerStatType)i;
            mainStats[i].Init(type);
        }

        if (data.AttackAmount > 1) attackAmount.text = $"x{data.AttackAmount}";
        else attackAmount.text = "";

        int abilIndex = 0;
        if (data.StatTypes.Length > 3)
        {
            for (int i = 3; i < data.StatTypes.Length; i++, abilIndex++)
            {
                EnumData.TowerStatType type = data.StatTypes[i];
                abilities[abilIndex].gameObject.SetActive(true);
                abilities[abilIndex].Init(type);
            }
        }

        if (data.BuffTypes != null)
            for (int i = 0; i < data.BuffTypes.Length; i++, abilIndex++)
            {
                EnumData.BuffType type = data.BuffTypes[i];
                abilities[abilIndex].gameObject.SetActive(true);
                abilities[abilIndex].Init(type);
            }

        if (data.DebuffTypes != null)
            for (int i = 0; i < data.DebuffTypes.Length; i++, abilIndex++)
            {
                EnumData.DebuffType type = data.DebuffTypes[i];
                abilities[abilIndex].gameObject.SetActive(true);
                abilities[abilIndex].Init(type);
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

        int abilIndex = 0;
        if (data.StatTypes.Length > 3)
        {
            for (int i = 3; i < data.StatTypes.Length; i++, abilIndex++)
            {
                EnumData.TowerStatType type = data.StatTypes[i];
                abilities[abilIndex].SetData(data.Stat(type));
            }
        }

        if (data.BuffTypes != null)
            for (int i = 0; i < data.BuffTypes.Length; i++, abilIndex++)
            {
                EnumData.BuffType type = data.BuffTypes[i];
                abilities[abilIndex].SetData(data.Buff(type));
            }

        if (data.DebuffTypes != null)
            for (int i = 0; i < data.DebuffTypes.Length; i++, abilIndex++)
            {
                EnumData.DebuffType type = data.DebuffTypes[i];
                abilities[abilIndex].SetData(data.Debuff(type));
            }
    }

    public void UpdateLanguage()
    {
        if (data != null) nameText.text = data.name;
    }
}
