using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] private RectTransform selectedTowerSlot;
    [SerializeField] private List<BuildTowerButton> buildTowerButtons;
    private int selectedTower = 0;
    private int maxCount;

    private void Update()
    {
        if (UIController.Instance.OpenScene != 1) return;

        float scroll = Input.GetAxis("ScrollWheel");
        if (scroll > 0)
        {
            selectedTower -= 1;
            if (selectedTower < 0) selectedTower = maxCount;
            SelectTower(selectedTower);
        }
        else if (scroll < 0)
        {
            selectedTower += 1;
            if (selectedTower > maxCount) selectedTower = 0;
            SelectTower(selectedTower);
        }

        for (int i = 0; i < 10; i++)
        {
            bool select = Input.GetButtonDown($"Select Tower {i + 1}");
            if (select)
            {
                SelectTower(i);
                break;
            }
        }
    }

    public void StartGame()
    {
        maxCount = -1;
        selectedTower = -1;

        selectedTowerSlot.gameObject.SetActive(false);
        for (int i = 0; i < buildTowerButtons.Count; i++)
        {
            buildTowerButtons[i].SetIndex(i);
            buildTowerButtons[i].SetItem(null);
        }
    }

    public void UpdateTowersInfo()
    {
        for (int i = 0; i < PlayerController.Instance.Towers.Count; i++)
        {
            Tower tower = PlayerController.Instance.Towers[i];

            buildTowerButtons[i].SetItem(tower);

            if (tower != null) maxCount = i;
        }
    }

    public void SelectTower(int index)
    {
        if (index == -1 || index > maxCount)
        {
            selectedTowerSlot.gameObject.SetActive(false);
            return;
        }

        selectedTowerSlot.gameObject.SetActive(true);
        selectedTower = index;
        selectedTowerSlot.anchoredPosition =
            ((RectTransform)buildTowerButtons[index].transform).anchoredPosition;

        buildTowerButtons[index].ReadyToBuildTower();
    }
}
