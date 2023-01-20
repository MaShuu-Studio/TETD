using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI probText;
    [SerializeField] private List<TowerInfoItem> items;

    private Dictionary<Grade, float> prob;

    private void Awake()
    {
        prob = new Dictionary<Grade, float>();
        prob.Add(Grade.NORMAL, 50);
        prob.Add(Grade.RARE, 80);
        prob.Add(Grade.HEROIC, 95);
        prob.Add(Grade.LEGENDARY, 100);

        probText.text =
            "NORMAL: " + prob[Grade.NORMAL] + "%\n" +
            "RARE: " + (prob[Grade.RARE] - prob[Grade.NORMAL]) + "%\n" +
            "HEROIC: " + (prob[Grade.HEROIC] - prob[Grade.RARE]) + "%\n" +
            "LEGENDARY: " + (prob[Grade.LEGENDARY] - prob[Grade.HEROIC]) + "%";
    }

    public void RerollAll()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Reroll(items[i]);
        }
    }
    public void Reroll(TowerInfoItem item)
    {
        float rand = Random.Range(0, 100f);
        Grade grade = Grade.NORMAL;
        if (rand <= prob[Grade.NORMAL]) grade = Grade.NORMAL;
        else if (rand <= prob[Grade.RARE]) grade = Grade.RARE;
        else if (rand <= prob[Grade.HEROIC]) grade = Grade.HEROIC;
        else grade = Grade.LEGENDARY;

        Element element = (Element) Random.Range(0, EnumArray.Elements.Length);

        Tower tower = TowerManager.GetRandomTower(element, grade);

        item.SetData(tower);
    }
}
