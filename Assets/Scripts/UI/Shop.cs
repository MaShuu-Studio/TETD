using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI probText;
    [SerializeField] private List<TowerInfoItem> items;

    private Dictionary<Grade, float> originProb;
    private Dictionary<Grade, float> prob;

    private void Awake()
    {
        originProb = new Dictionary<Grade, float>();
        originProb.Add(Grade.NORMAL, 50);
        originProb.Add(Grade.RARE, 30);
        originProb.Add(Grade.HEROIC, 15);
        originProb.Add(Grade.LEGENDARY, 5);

        prob = new Dictionary<Grade, float>(originProb);
    }

    public void UpdateProb()
    {
        float mul = PlayerController.Instance.BonusProb;

        prob[Grade.RARE] = originProb[Grade.RARE] * mul;
        prob[Grade.HEROIC] = originProb[Grade.HEROIC] * mul;
        prob[Grade.LEGENDARY] = originProb[Grade.LEGENDARY] * mul;

        prob[Grade.NORMAL] = 100 - (prob[Grade.RARE] + prob[Grade.HEROIC] + prob[Grade.LEGENDARY]);

        probText.text =
            "NORMAL: " + string.Format("{0:0.##}", prob[Grade.NORMAL]) + "%\n" +
            "RARE: " + string.Format("{0:0.##}", prob[Grade.RARE]) + "%\n" +
            "HEROIC: " + string.Format("{0:0.##}", prob[Grade.HEROIC]) + "%\n" +
            "LEGENDARY: " + string.Format("{0:0.##}", prob[Grade.LEGENDARY]) + "%";
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

        if (rand <= prob[Grade.LEGENDARY]) grade = Grade.LEGENDARY;
        else if (rand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC]) grade = Grade.HEROIC;
        else if (rand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC] + prob[Grade.RARE]) grade = Grade.RARE;

        Tower tower = TowerManager.GetRandomTower(item.SelectedElement, grade);

        item.SetData(tower);
    }
}
