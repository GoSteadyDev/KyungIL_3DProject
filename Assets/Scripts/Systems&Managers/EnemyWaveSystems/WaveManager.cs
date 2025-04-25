using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    [SerializeField] private List<WaveData> waveDatas;
    [SerializeField] private EnemySpawner spawner;

    private int currentWaveIndex = 0;
    private bool isWaveRunning = false;
    private int killedEnemyCount = 0;
    
    public bool IsWaveRunning => isWaveRunning;
    // 읽기 전용으로 뒀던 프로퍼티를 지우고,
    // 외부에서 값을 지정할 수 있게 메서드로 바꿔 주세요.
    public void SetWaveIndex(int index)
    {
        currentWaveIndex = Mathf.Clamp(index, 0, waveDatas.Count);
        // UI 업데이트가 필요하면 여기서 ShowWaveInfo 호출해도 됩니다.
    }

    public int CurrentWaveIndex => currentWaveIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (KillEventSystem.Instance != null)
        {
            KillEventSystem.Instance.OnKill += HandleKillEvent;
        }
    }

    private void OnDisable()
    {
        if (KillEventSystem.Instance != null)
        {
            KillEventSystem.Instance.OnKill -= HandleKillEvent;
        }
    }
    
    public void StartNextWave()
    {
        killedEnemyCount = 0;
        
        // 웨이브가 이미 진행 중이거나 마지막 웨이브를 넘었거나 적이 남아있으면 리턴
        if (isWaveRunning || spawner.HasAliveEnemies() || currentWaveIndex >= waveDatas.Count)
            return;
        
        NotificationService.Notify("Wave started! They are coming—prepare for battle.");
        
        UIManager.Instance.ShowWaveInfo(
            currentWaveIndex + 1,
            killedEnemyCount,
            CurrentWaveEnemyCount
        );
        
        StartCoroutine(RunWave(waveDatas[currentWaveIndex]));
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

        NotificationService.Notify("All enemies have been destroyed. You can start next wave.");
        
        isWaveRunning = false;

        // ✅ 웨이브 종료 후에 증가
        currentWaveIndex++;
        
        // 모든 웨이브를 끝냈다면 바로 승리 처리
        if (currentWaveIndex >= waveDatas.Count)
        {
            GameManager.Instance.OnGameWin();
        }
    }
    
    private void HandleKillEvent(KillEvent killEvent)
    {
        if (!isWaveRunning) return;
        
        killedEnemyCount++;
        
        UIManager.Instance.ShowWaveInfo(
            currentWaveIndex + 1,killedEnemyCount,
            CurrentWaveEnemyCount
        );
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
