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

        // 이펙트 처리 등은 Castle 객체가 맡고
        UIManager.Instance.ShowGameOver("Loose !");
    }

    public void OnGameWin()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Time.timeScale = 0f;

        Debug.Log("Game Clear - 승리!");
        
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

