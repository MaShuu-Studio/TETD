using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : TowerInfo
{
    [Space]
    [SerializeField] private Toggle[] priorityToggles;
    private RectTransform rectTransform;
    public RectTransform RectTransform { get { return rectTransform; } }

    private TowerObject selectedTower;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void SetData(Tower data)
    {
        base.SetData(data);

        selectedTower = TowerController.Instance.SelectedTower;
        int index = (int)selectedTower.Priority;
        priorityToggles[index].isOn = true;
    }

    // ��� �۵��� ValueChanged�� ���� ����
    public void SetPriority(bool b)
    {
        // �ϳ��� �۵��ϵ��� false�� return
        if (b == false) return;

        for (int i = 0; i < priorityToggles.Length; i++)
        {
            if (priorityToggles[i].isOn)
            {
                selectedTower.ChangePriority((EnumData.AttackPriority)i);
                break;
            }
        }
    }
}
