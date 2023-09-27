using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using EnumData;

public class MapEditorMapProperty : MonoBehaviour
{
    [SerializeField] private CustomDropdown elementDropdown;
    [SerializeField] private Button removeButton;
    [SerializeField] private Image elementImage;
    [SerializeField] private TMP_InputField[] inputs;

    private MapProperty prop;
    private MapEditorDataPanel dataPanel;

    public void SetProp(int index, MapProperty prop, MapEditorDataPanel dataPanel)
    {
        this.prop = prop;
        this.dataPanel = dataPanel;

        removeButton.onClick.AddListener(() => dataPanel.RemoveData(index));

        elementImage.sprite = SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)prop.element);
        inputs[0].text = prop.atk.ToString();
        inputs[1].text = prop.atkSpeed.ToString();
        inputs[2].text = prop.hp.ToString();
        inputs[3].text = prop.speed.ToString();

        elementDropdown.onValueChanged.AddListener(index => ChangeElement(index));
        inputs[0].onValueChanged.AddListener(str => int.TryParse(str, out prop.atk));
        inputs[1].onValueChanged.AddListener(str => int.TryParse(str, out prop.atkSpeed));
        inputs[2].onValueChanged.AddListener(str => int.TryParse(str, out prop.hp));
        inputs[3].onValueChanged.AddListener(str => int.TryParse(str, out prop.speed));
    }

    public void UpdateElement(int index, List<Element> elements)
    {
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => dataPanel.RemoveData(index));

        elementDropdown.Clear();
        elementDropdown.AddOption(new CustomDropdownOption($"{(int)prop.element}", SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)prop.element)));
        for (int i = 0; i < elements.Count; i++)
        {
            elementDropdown.AddOption(new CustomDropdownOption($"{(int)elements[i]}", SpriteManager.GetSpriteWithNumber(SpriteManager.ETCDataNumber.ELEMENT, (int)elements[i])));
        }
    }

    private void ChangeElement(int index)
    {
        prop.element = (Element)int.Parse(elementDropdown.Options[index].str);
        dataPanel.UpdateElements();
    }
}
