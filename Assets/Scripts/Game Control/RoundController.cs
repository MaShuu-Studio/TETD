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

    public float Difficulty { get { return amountDiff; } }
    private float amountDiff;
    private float timeDiff;
    public bool IsEnd { get { return isEnd; } }
    private bool isEnd;

    public void Init(string mapName, List<DifficultyType> difficulties)
    {
        isEnd = false;

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
        if (data.data.Count == curRound)
        {
            isEnd = true;
            timerCoroutine = null;
        }
        else
        {
            EachRound nextRound = data.data[curRound];
            UIController.Instance.NextRoundInfo(nextRound);
            float time = 0;

            while (time < (5 * timeDiff))
            {
                if (GameController.Instance.Paused)
                {
                    yield return null;
                    continue;
                }
                time += Time.deltaTime;
                yield return null;
            }

            if (curRound == cur)
                StartRound();
        }
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
                    if (GameController.Instance.Paused)
                    {
                        yield return null;
                        continue;
                    }
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        }

        timerCoroutine = NextRoundTimer(curRound);
        StartCoroutine(timerCoroutine);
    }
}
