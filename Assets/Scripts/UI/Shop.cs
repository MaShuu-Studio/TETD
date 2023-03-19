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

    public void Init()
    {
        originProb = new Dictionary<Grade, float>();
        originProb.Add(Grade.NORMAL, 50);
        originProb.Add(Grade.RARE, 30);
        originProb.Add(Grade.HEROIC, 15);
        originProb.Add(Grade.LEGENDARY, 5);

        prob = new Dictionary<Grade, float>(originProb);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Init();
        }
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
        Tower tower = null;
        bool contains = false;

        do
        {
            float gradeRand = Random.Range(0, 100f);
            Grade grade = Grade.NORMAL;
            if (gradeRand <= prob[Grade.LEGENDARY]) grade = Grade.LEGENDARY;
            else if (gradeRand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC]) grade = Grade.HEROIC;
            else if (gradeRand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC] + prob[Grade.RARE]) grade = Grade.RARE;

            int element = item.SelectedElement;
            if (element < 0) element = Random.Range(0, EnumArray.Elements.Length);
            int count = PlayerController.Instance.UsableTowers[element, (int)grade].Count;

            // 사용 가능한 타워가 없으면 등급 다운.
            // 해당 부분은 실시간으로 표기가 필요할 듯.
            while (count == 0)
            {
                grade--;
                count = PlayerController.Instance.UsableTowers[element, (int)grade].Count;
            }

            int towerRand = Random.Range(0, count);
            int id = PlayerController.Instance.UsableTowers[element, (int)grade][towerRand];

            tower = TowerManager.GetTower(id);

            // 현재 뽑은 목록 중에 존재한다면 스킵.
            contains = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Data != null && items[i].Data.id == tower.id)
                {
                    contains = true;
                    break;
                }
            }
        } while (contains);

        item.SetData(tower);
    }

    public void UpdateLanguage()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdateLanguage();
        }
    }
}
