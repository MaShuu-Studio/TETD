using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI probText;
    [SerializeField] private Transform pubRemainParent;
    [SerializeField] private ShopRemainInfo remainPrefab;
    private List<ShopRemainInfo> remains;
    [SerializeField] private List<TowerInfoItem> items;
    [SerializeField] private GameObject remainInfo;

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
            items[i].Init();

        remains = new List<ShopRemainInfo>();
        for (int e = 0; e < EnumArray.Elements.Length; e++)
        {
            ShopRemainInfo remain = Instantiate(remainPrefab);
            remain.transform.SetParent(pubRemainParent);
            remains.Add(remain);

            remain.Init(e);
        }
    }

    public void StartGame()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].StartGame();

        UpdateProb();
        UpdateAmount();
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

    public void UpdateAmount()
    {
        for (int e = 0; e < remains.Count; e++)
        {
            string str = "";
            for (int g = 0; g < EnumArray.Grades.Length; g++)
            {
                str += PlayerController.Instance.UsableTowers[e, g].Count + "\n";
            }
            remains[e].SetData(str);
        }
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
            Grade grade = GetGrade(gradeRand);

            int element = item.SelectedElement;
            if (element < 0) element = Random.Range(0, EnumArray.Elements.Length);
            int count = PlayerController.Instance.UsableTowers[element, (int)grade].Count;

            while (count == 0)
            {
                // 속성 선택을 하지 않은 경우 등급을 유지한 채로 속성을 변경함.
                if (item.SelectedElement < 0)
                {
                    int e = element;
                    while (element == e) e = Random.Range(0, EnumArray.Elements.Length);
                    element = e;
                }
                else
                {
                    // 해당 등급의 타워가 없다면 리롤
                    Grade g = grade;
                    while (grade == g)
                    {
                        gradeRand = Random.Range(0, 100f);
                        g = GetGrade(gradeRand);
                    }
                    grade = g;
                }

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

    private Grade GetGrade(float rand)
    {
        Grade grade = Grade.NORMAL;
        if (rand <= prob[Grade.LEGENDARY]) grade = Grade.LEGENDARY;
        else if (rand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC]) grade = Grade.HEROIC;
        else if (rand <= prob[Grade.LEGENDARY] + prob[Grade.HEROIC] + prob[Grade.RARE]) grade = Grade.RARE;

        return grade;
    }

    public void OnOffRemainInfo()
    {
        remainInfo.SetActive(!remainInfo.activeSelf);
    }

    public void UpdateLanguage()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdateLanguage();
        }
    }
}
