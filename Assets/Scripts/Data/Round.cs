using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data;

public class Round
{
    public string mapName;
    public List<EachRound> data;

    public Round(RoundData data)
    {
        mapName = data.mapName;

        this.data = new List<EachRound>();
        for (int i = 0; i < data.data.Count; i++)
        {
            EachRoundData eachData = data.data[i];
            EachRound eachRound = new EachRound(eachData.units, eachData.amounts);
            this.data.Add(eachRound);
        }
    }
}
public class EachRound
{
    public Dictionary<int, int> unitData;

    public EachRound(List<int> ids, List<int> amounts)
    {
        unitData = new Dictionary<int, int>();

        int count = Mathf.Min(ids.Count, amounts.Count);

        for (int i = 0; i < count; i++)
        {
            unitData.Add(ids[i], amounts[i]);
        }
    }
}

[Serializable]
public class EachRoundData
{
    public List<int> units;
    public List<int> amounts;
}

[Serializable]
public class RoundData
{
    public string mapName;
    public List<EachRoundData> data;
}