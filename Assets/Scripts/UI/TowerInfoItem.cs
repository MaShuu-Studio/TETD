using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfoItem : TowerInfoCard
{
    [Space]
    [SerializeField] private TMP_Dropdown elementDropdown;

    public int SelectedElement { get { return selectedElement; } }
    private int selectedElement = -1;

    public void Init()
    {
        selectedElement = -1;
        if (elementDropdown != null)
        { 
            elementDropdown.value = 0;

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < EnumArray.Elements.Length; i++)
            {
                Element e = EnumArray.Elements[i];
                string oName = EnumArray.ElementStrings[e].ToUpper();
                Sprite oSprite = SpriteManager.GetSprite($"Element{i}");
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(oName, oSprite);
                options.Add(option);
            }
            elementDropdown.AddOptions(options);
            elementDropdown.template.sizeDelta = new Vector2(elementDropdown.template.sizeDelta.x, 75 * (1 + options.Count));            
        }
    }

    public void SelectElement(int index)
    {
        selectedElement = index - 1;
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        costText.text = PlayerController.Cost(data.cost).ToString();
    }
}
