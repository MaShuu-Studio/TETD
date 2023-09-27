using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapEditorRoundUnitInfo : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown unitsDropdown;
    [SerializeField] private Image unitImage;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private Button removeButton;

    private int id;
    private int amount;

    public void SetIcon(int id, int amount, MapEditorRoundInfo roundInfo)
    {
        this.id = id;
        this.amount = amount;

        Sprite sprite = SpriteManager.GetSprite(id);
        unitImage.sprite = sprite;
        unitImage.rectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) * 3;
        amountInput.text = amount.ToString();

        removeButton.onClick.AddListener(() => roundInfo.RemoveUnit(this.id));
        amountInput.onValueChanged.AddListener(str =>
        {
            int.TryParse(str, out this.amount);
            roundInfo.ChangeUnitAmount(this.id, this.amount);
        });

        unitsDropdown.onValueChanged.AddListener(index =>
        {
            string str = unitsDropdown.options[index].text;
            int id;
            int.TryParse(str, out id);
            Sprite sprite = SpriteManager.GetSprite(id);
            unitImage.sprite = sprite;
            roundInfo.ChangeUnit(this.id, id);
            this.id = id;
            roundInfo.UpdateUnitList();
        });
    }

    public void UpdateUnitList(List<int> units)
    {
        unitsDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData(id.ToString(), null));
        foreach (var key in units)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(key.ToString(), null);
            options.Add(option);
        }
        unitsDropdown.AddOptions(options);
    }
}
