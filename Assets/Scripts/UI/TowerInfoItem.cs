using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnumData;

public class TowerInfoItem : TowerInfo
{
    [Space]
    private Image gradeImage;
    [SerializeField] protected TextMeshProUGUI costText;

    [Space]
    [SerializeField] private TMP_Dropdown elementDropdown;
    public int SelectedElement { get { return selectedElement; } }
    private int selectedElement = -1;

    private void Awake()
    {
        gradeImage = GetComponent<Image>();
    }

    public override void SetData(Tower data)
    {
        base.SetData(data);

        // 간단하게 등급 구분을 먼저 색으로만
        Color c = data.GradeColor;

        if (gradeImage == null) gradeImage = GetComponent<Image>();
        gradeImage.color = c;
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        costText.text = PlayerController.Cost(data.cost).ToString();
    }

    public void Init()
    {
        selectedElement = -1;
        if (elementDropdown != null)
        { 
            elementDropdown.value = -1;

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < EnumArray.Elements.Length; i++)
            {
                Element e = EnumArray.Elements[i];
                string oName = EnumArray.ElementStrings[e].ToUpper();
                Sprite oSprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, i);
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(oName, oSprite);
                options.Add(option);
            }
            elementDropdown.AddOptions(options);
            elementDropdown.template.sizeDelta = new Vector2(elementDropdown.template.sizeDelta.x, 75 * (1 + options.Count));            
        }
    }

    public void SelectElement()
    {
        selectedElement = elementDropdown.value - 1;
    }
}
