using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<TowerInfoItem> items;

    public void RerollAll()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Reroll(items[i]);
        }
    }
    public void Reroll(TowerInfoItem item)
    {
        // Ȯ���� ���� ������ġ ���� ����
        int amount = TowerManager.Keys.Count;

            int rand = Random.Range(0, amount);

            int id = TowerManager.Keys[rand];
            Tower tower = TowerManager.GetTower(id);

            item.SetData(tower);
    }
}
