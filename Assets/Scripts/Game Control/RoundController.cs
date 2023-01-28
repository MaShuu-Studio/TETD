using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class RoundController : MonoBehaviour
{
    public static RoundController Instance { get { return instance; } }
    private static RoundController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private IEnumerator timerCoroutine;
    private IEnumerator spawnCoroutine;
    private Round data;
    private int curRound;

    private float amountDiff;
    private float timeDiff;

    public void Init(string mapName, List<DifficultyType> difficulties)
    {
        amountDiff = 1;
        timeDiff = 1;

        if (difficulties.Contains(DifficultyType.AMOUNT)) amountDiff = 1.5f;
        if (difficulties.Contains(DifficultyType.TIME)) timeDiff = 0.5f;

        data = RoundManager.GetRound(mapName);
        curRound = 0;

        timerCoroutine = NextRoundTimer(curRound);
        StartCoroutine(timerCoroutine);
    }

    public void StartRound()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        EachRound round = data.data[curRound++];

        UIController.Instance.NextRoundInfo(null);

        spawnCoroutine = MobSpawn(round);
        StartCoroutine(spawnCoroutine);
    }

    IEnumerator NextRoundTimer(int cur)
    {
        EachRound nextRound = data.data[curRound];
        UIController.Instance.NextRoundInfo(nextRound, amountDiff);
        float time = 0;

        while (time < (5 * timeDiff))
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (curRound == cur)
            StartRound();
    }

    IEnumerator MobSpawn(EachRound round)
    {
        foreach (int id in round.unitData.Keys)
        {
            for (int i = 0; i < round.unitData[id] * amountDiff; i++)
            {
                EnemyController.Instance.AddEnemy(id);

                float time = 0;
                while (time < 0.3f)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        }

        timerCoroutine = NextRoundTimer(curRound);
        StartCoroutine(timerCoroutine);
    }
}
