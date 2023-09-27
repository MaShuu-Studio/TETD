using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorRoundInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private MapEditorRoundUnitInfo unitInfoPrefab;
    [SerializeField] private RectTransform unitInfoParent;
    [SerializeField] private Button removeButton;
    [SerializeField] private Transform addUnitButton;

    private Round round;
    private Dictionary<int, MapEditorRoundUnitInfo> unitInfos;

    private int number;
    private MapEditorDataPanel dataPanel;

    public void SetRoundInfo(int number, Round round, MapEditorDataPanel dataPanel)
    {
        this.number = number;
        this.round = round;
        this.dataPanel = dataPanel;

        roundNumberText.text = number.ToString();
        removeButton.onClick.AddListener(() => dataPanel.RemoveData(number));

        unitInfoPrefab.gameObject.SetActive(false);
        unitInfos = new Dictionary<int, MapEditorRoundUnitInfo>();
        foreach (var kv in round.unitData)
        {
            MapEditorRoundUnitInfo unitInfo = Instantiate(unitInfoPrefab);
            unitInfo.SetIcon(kv.Key, kv.Value, this);
            unitInfo.transform.SetParent(unitInfoParent);
            unitInfo.transform.localScale = Vector3.one;
            unitInfo.gameObject.SetActive(true);

            unitInfos.Add(kv.Key, unitInfo);
        }
        addUnitButton.SetAsLastSibling();

        unitInfoParent.sizeDelta = new Vector2(144 * (unitInfos.Count + 1), 165);
        UpdateUnitList();
    }

    public void AddUnit()
    {
        int id = GetRemainUnitList()[0];
        AddUnit(id);
    }
    public void AddUnit(int id, bool update = true)
    {
        MapEditorRoundUnitInfo unitInfo = Instantiate(unitInfoPrefab);
        unitInfo.SetIcon(id, 1, this);
        unitInfo.transform.SetParent(unitInfoParent);
        unitInfo.transform.localScale = Vector3.one;
        unitInfo.gameObject.SetActive(true);

        round.unitData.Add(id, 1);
        unitInfos.Add(id, unitInfo);
        addUnitButton.SetAsLastSibling();

        unitInfoParent.sizeDelta = new Vector2(144 * (unitInfos.Count + 1), 165);

        if (update) UpdateUnitList();
    }

    public void ChangeUnitAmount(int id, int amount)
    {
        round.unitData[id] = amount;
    }
    public void ChangeUnit(int id, int newId)
    {
        RemoveUnit(id, false);
        AddUnit(newId, false);
    }

    public void RemoveUnit(int id, bool update = true)
    {
        round.unitData.Remove(id);
        Destroy(unitInfos[id].gameObject);
        unitInfos.Remove(id);
        addUnitButton.SetAsLastSibling();

        unitInfoParent.sizeDelta = new Vector2(144 * (unitInfos.Count + 1), 165);

        if (update) UpdateUnitList();
    }

    public void UpdateUnitList()
    {
        List<int> list = GetRemainUnitList();
        foreach (var info in unitInfos.Values)
        {
            info.UpdateUnitList(list);
        }
    }

    public void UpdateNumber(int number)
    {
        this.number = number;
        roundNumberText.text = number.ToString();

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() => dataPanel.RemoveData(number));
    }

    public List<int> GetRemainUnitList()
    {
        List<int> list = new List<int>();
        list.AddRange(EnemyManager.Keys);
        list.AddRange(CustomDataManager.EnemyKeys);
        foreach (var key in round.unitData.Keys)
        {
            list.Remove(key);
        }

        return list;
    }
}
