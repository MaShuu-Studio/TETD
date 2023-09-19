using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EnumData;

public class UnitEditorAbility : MonoBehaviour
{
    [SerializeField] private CustomDropdown towerAbilityDropdown;
    [SerializeField] private TMP_InputField[] towerAbilityInputs;

    public void Init(UnitEditor editor)
    {
        towerAbilityDropdown.onValueChanged.AddListener(i => UpdateData(i));
        towerAbilityDropdown.onValueChanged.AddListener(i => editor.AblityChanged());
        towerAbilityDropdown.onValueChanged.AddListener(i => editor.UpdateDataToPosterAbility());

        for (int i = 0; i < towerAbilityInputs.Length; i++)
        {
            towerAbilityInputs[i].onValueChanged.AddListener(s => editor.UpdateDataToPosterAbility());
        }

        List<CustomDropdownOption> options = new List<CustomDropdownOption>();
        for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
        {
            AbilityType type = EnumArray.AbilityTypes[i];
            string oName = EnumArray.AbilityTypeStrings[type];
            Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.TOWERABILITY, (int)type);
            CustomDropdownOption option = new CustomDropdownOption(oName, oSprite);
            options.Add(option);
        }

        float width = ((options.Count >= 5) ? 5 : options.Count) * 87.5f;
        float height = (100f / 3f * 2f) * (1 + (int)(options.Count / 5)) + 25;

        towerAbilityDropdown.template.sizeDelta = new Vector2(width, height);
        towerAbilityDropdown.AddOptions(options);
    }

    private void UpdateData(int index)
    {
        if (index == 0)
        {
            for (int i = 0; i < towerAbilityInputs.Length; i++)
            {
                towerAbilityInputs[i].gameObject.SetActive(false);
                towerAbilityInputs[i].text = "";
            }
        }
        else
        {
            AbilityType type = ChangeValueToAbilityType(index);
            towerAbilityInputs[0].gameObject.SetActive(true);
            towerAbilityInputs[0].text = "";
            if (Tower.IsBuff(type))
            {
                towerAbilityInputs[1].gameObject.SetActive(true);
                towerAbilityInputs[1].text = "";
            }
            else
                towerAbilityInputs[1].gameObject.SetActive(false);

            if (Tower.IsBuff(type) || Tower.IsDebuff(type))
            {
                towerAbilityInputs[2].gameObject.SetActive(true);
                towerAbilityInputs[2].text = "";
            }
            else
                towerAbilityInputs[2].gameObject.SetActive(false);
        }
    }
    public void UpdateData(TowerAbility ability)
    {
        if (ability == null)
        {
            towerAbilityDropdown.value = 0;
            for (int i = 0; i < towerAbilityInputs.Length; i++)
            {
                towerAbilityInputs[i].gameObject.SetActive(false);
                towerAbilityInputs[i].text = "";
            }
        }
        else
        {
            AbilityType type = (AbilityType)ability.type;
            towerAbilityDropdown.value = ChangeAbilityToValue(type);

            towerAbilityInputs[0].gameObject.SetActive(true);
            towerAbilityInputs[0].text = ability.value.ToString();
            if (Tower.IsBuff(type))
            {
                towerAbilityInputs[1].gameObject.SetActive(true);
                towerAbilityInputs[1].text = ability.atkSpeed.ToString();
            }
            else
                towerAbilityInputs[1].gameObject.SetActive(false);

            if (Tower.IsBuff(type) || Tower.IsDebuff(type))
            {
                towerAbilityInputs[2].gameObject.SetActive(true);
                towerAbilityInputs[2].text = ability.time.ToString();
            }
            else
                towerAbilityInputs[2].gameObject.SetActive(false);
        }
    }

    public TowerAbility GetData()
    {
        TowerAbility ability = new TowerAbility();
        int index = towerAbilityDropdown.value;

        if (index == 0) return null;

        ability.type = (int)EnumArray.AbilityTypes[index - 1];
        float.TryParse(towerAbilityInputs[0].text, out ability.value);
        float.TryParse(towerAbilityInputs[1].text, out ability.atkSpeed);
        float.TryParse(towerAbilityInputs[2].text, out ability.time);

        return ability;
    }

    private int ChangeAbilityToValue(AbilityType type)
    {
        for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
        {
            if (type == EnumArray.AbilityTypes[i]) return i + 1;
        }
        return 0;
    }

    private AbilityType ChangeValueToAbilityType(int index)
    {
        index = index - 1;
        for (int i = 0; i < EnumArray.AbilityTypes.Length; i++)
        {
            if (i == index) return EnumArray.AbilityTypes[i];
        }
        return 0;
    }
}
