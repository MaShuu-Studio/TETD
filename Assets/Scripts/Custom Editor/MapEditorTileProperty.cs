using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorTileProperty : MonoBehaviour
{
    [SerializeField] private Image tileImage;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Button removeButton;
    [SerializeField] private TextMeshProUGUI posText;
    [SerializeField] private TMP_InputField[] inputs;

    private TileProperty prop;
    private MapEditorDataPanel dataPanel;

    public void SetProp(int index, Vector3Int pos, TileProperty prop, MapEditorDataPanel dataPanel)
    {
        this.prop = prop;
        this.dataPanel = dataPanel;

        removeButton.onClick.AddListener(() => dataPanel.RemoveData(pos));

        tileImage.sprite = TileManager.GetFlag("SPECIALZONE").Base.sprite;

        posText.text = $"POS - X: {pos.x}, Y: {pos.y}";
        numberText.text = $"{index}";

        inputs[0].text = prop.atk.ToString();
        inputs[1].text = prop.atkSpeed.ToString();
        inputs[2].text = prop.range.ToString();

        inputs[0].onValueChanged.AddListener(str => int.TryParse(str, out prop.atk));
        inputs[1].onValueChanged.AddListener(str => int.TryParse(str, out prop.atkSpeed));
        inputs[2].onValueChanged.AddListener(str => int.TryParse(str, out prop.range));
    }

    public void UpdateNumber(int index)
    {
        numberText.text = $"{index}";
        transform.SetAsLastSibling();
    }
}
