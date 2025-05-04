using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private Canvas introCanvas;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;

    private bool hasStarted = false;
    
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
        var data = SaveSystem.PendingLoad;

        // 💡 세이브가 있을 경우 → 인트로 생략
        if (data != null)
        {
            introCanvas.gameObject.SetActive(false); // 인트로 UI 강제 비활성화
            hasStarted = true;

            ApplyLoad(data);
            SaveSystem.PendingLoad = null;
            return;
        }

        // 🟢 새 게임일 경우만 인트로 실행
        StartCoroutine(BlinkPressAnyKey());
    }
    
    private IEnumerator BlinkPressAnyKey()
    {
        while (!hasStarted)
        {
            float time = 0f;
            float duration = 3f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, Mathf.PingPong(time * 2f, 1f)); // 부드럽게 깜빡임
                var color = pressAnyKeyText.color;
                color.a = alpha;
                pressAnyKeyText.color = color;

                yield return null;
            }
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
        if (hasStarted == false && Input.anyKeyDown)
        {
            hasStarted = true;
            introCanvas.gameObject.SetActive(false); // Canvas 전체 비활성화
            return;
        }
        
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

