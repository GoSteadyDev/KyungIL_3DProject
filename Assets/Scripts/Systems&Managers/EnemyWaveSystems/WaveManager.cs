using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveDatas;
    [SerializeField] private EnemySpawner spawner;

    private int currentWaveIndex = 0;
    private bool isWaveRunning = false;

    private void OnEnable()
    {
        KillEventSystem.Instance.OnKill += HandleKillEvent;
    }

    private void OnDisable()
    {
        KillEventSystem.Instance.OnKill -= HandleKillEvent;
    }
    
    public void StartNextWave()
    {
        // 웨이브가 이미 진행 중이거나 마지막 웨이브를 넘었거나 적이 남아있으면 리턴
        if (isWaveRunning || spawner.HasAliveEnemies() || currentWaveIndex >= waveDatas.Count)
            return;

        UIManager.Instance.ShowWaveInfo(
            currentWaveIndex + 1,
            spawner.GetAliveEnemyCount(),
            waveDatas[currentWaveIndex].GetTotalEnemyCount
        );
        
        StartCoroutine(RunWave(waveDatas[currentWaveIndex]));
        currentWaveIndex++;
    }

    private IEnumerator RunWave(WaveData waveData)
    {
        isWaveRunning = true;

        foreach (var info in waveData.enemyList)
        {
            for (int i = 0; i < info.enemyCount; i++)
            {
                spawner.Spawn(info.enemyData);
                yield return new WaitForSeconds(waveData.spawnInterval);
            }
        }

        yield return new WaitUntil(() => spawner.HasAliveEnemies() == false);

        isWaveRunning = false;

        // 모든 웨이브를 끝냈다면 바로 승리 처리
        if (currentWaveIndex >= waveDatas.Count)
        {
            GameManager.Instance.OnGameWin();
        }
    }
    
    private void HandleKillEvent(KillEvent killEvent)
    {
        if (!isWaveRunning) return;

        int alive = spawner.GetAliveEnemyCount(); // 아래에서 정의
        int total = CurrentWaveEnemyCount;        // 기존 WaveData에서 계산

        UIManager.Instance.ShowWaveInfo(currentWaveIndex + 1, alive, total);
    }
    
    public int CurrentWaveEnemyCount
    {
        get
        {
            if (currentWaveIndex >= waveDatas.Count) return 0;
            return waveDatas[currentWaveIndex].GetTotalEnemyCount;
        }
    }
}
