using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] protected Image elementImage;
    [SerializeField] protected RectTransform iconSlot;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TowerStatItem[] mainStats;
    [SerializeField] protected TextMeshProUGUI attackAmount;
    [SerializeField] protected TowerAbilityItem[] abilities;
    [SerializeField] protected TextMeshProUGUI nameText;

    protected Tower data;
    public Tower Data { get { return data; } }

    public virtual void SetData(Tower data)
    {
        this.data = data;

        nameText.text = data.name;
        iconImage.sprite = SpriteManager.GetSprite(data.id);
        if (iconSlot != null)
        {
            iconSlot.sizeDelta
                = new Vector2(iconImage.sprite.texture.width, iconImage.sprite.texture.height) / 24 * CameraController.ReferencePPU;
        }

        // UI Icon들도 id를 붙이고 SpriteManager에서 관리 예정
        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)data.element);

        for (int i = 0; i < mainStats.Length; i++)
        {
            TowerStatType type = (TowerStatType)i;
            mainStats[i].Init(type);
        }

        if (data.AttackAmount > 1) attackAmount.text = $"x{data.AttackAmount}";
        else attackAmount.text = "";

        bool isBuffTower = (data.Stat(TowerStatType.DAMAGE) != 0);
        mainStats[0].gameObject.SetActive(isBuffTower);
        mainStats[1].gameObject.SetActive(isBuffTower);

        int index = 0;
        if (data.AbilityTypes != null && data.AbilityTypes.Count > 0)
        {
            for (index = 0; index < data.AbilityTypes.Count; index++)
            {
                AbilityType type = data.AbilityTypes[index];
                abilities[index].gameObject.SetActive(true);
                abilities[index].Init(type);
            }
        }

        for (; index < abilities.Length; index++)
        {
            abilities[index].gameObject.SetActive(false);
        }
        UpdateInfo();
    }

    public virtual void UpdateInfo()
    {
        for (int i = 0; i < mainStats.Length; i++)
        {
            TowerStatType type = (TowerStatType)i;
            mainStats[i].SetData(data.Stat(type));
        }

        if (data.AbilityTypes != null && data.AbilityTypes.Count > 0)
        {
            for (int i = 0; i < data.AbilityTypes.Count; i++)
            {
                AbilityType type = data.AbilityTypes[i];
                abilities[i].SetData(data.Ability(type));
            }
        }
    }

    public virtual void UpdateLanguage()
    {
        if (data != null) nameText.text = data.name;
    }
}
