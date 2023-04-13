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
        if (elementDropdown != null) elementDropdown.value = 0;
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
