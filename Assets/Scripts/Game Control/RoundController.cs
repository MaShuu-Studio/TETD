using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init(string mapName)
    {
        data = RoundManager.GetRound(mapName);
        curRound = 0;

        timerCoroutine = NextRoundTimer(curRound);
        StartCoroutine(timerCoroutine);
    }

    public void StartRound()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        EachRound round = data.data[curRound++];

        spawnCoroutine = MobSpawn(round);
        StartCoroutine(spawnCoroutine);
    }

    IEnumerator NextRoundTimer(int cur)
    {
        float time = 0;

        while (time < 5)
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
            for (int i = 0; i < round.unitData[id]; i++)
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
