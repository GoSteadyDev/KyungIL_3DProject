using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // PendingLoad가 null이 아니면 복원 시도
        var data = SaveSystem.PendingLoad;
        if (data != null)
        {
            ApplyLoad(data);
            SaveSystem.PendingLoad = null;
        }
    }

    private void ApplyLoad(GameSaveData data)
    {
        // 1) 골드
        ResourceManager.Instance.SetGold(data.gold);
        // 2) 웨이브
        WaveManager.Instance.SetWaveIndex(data.waveIndex);
        // 3) 타워
        BuildingSystem.Instance.ClearAllTowers();
        foreach (var ts in data.towers)
            BuildingSystem.Instance.SpawnTowerFromSave(ts);
        // (유닛 복원도 여기에)
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void OnCastleDestroyed()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Time.timeScale = 0f;

        UIManager.Instance.ShowGameOver("Loose !");
    }

    public void OnGameWin()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Time.timeScale = 0f;
        
        UIManager.Instance.ShowGameOver("Win !");  // true는 승리
    }
    
    public void OnExitClick()
    {
        Time.timeScale = 1f;

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 유니티 에디터에서 중지
    #else
        Application.Quit(); // 빌드된 게임에서 종료
    #endif
    }
    
    public void TogglePause()
    {
        if (IsGameOver) return;

        bool nowPaused = Time.timeScale == 1f;
        Time.timeScale = nowPaused ? 0f : 1f;

        // 이벤트 대신 직접 UIManager에게 알려 주기
        UIManager.Instance.ShowPauseUI(nowPaused);
    }
}

