using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pausePanel;      // PausePanel 전체
    [SerializeField] private GameObject settingsPanel;   // Settings 서브패널

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button quitButton;

    [Header("Other Systems")]
    [Tooltip("Hierarchy에서 Main Camera에 붙어 있는 스크립트")]
    [SerializeField] private CameraControlSystem cameraControl;
    
    [Header("Sliders")]
    [SerializeField] private Slider cameraSpeedSlider;
    [SerializeField] private Slider bgmVolumeSlider;  // 0~1

    [Header("Audio Settings")]
    [SerializeField] private AudioSource bgmSource;       // BGM용 AudioSource
    
    private void Awake()
    {
        // 버튼 콜백 설정
        resumeButton.onClick.AddListener(OnResume);
        saveButton.onClick.AddListener(OnSave);
        loadButton.onClick.AddListener(OnLoad);
        settingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        quitButton.onClick.AddListener(OnQuit);

        // 슬라이더 초기화 & 바인딩
        cameraSpeedSlider.value = cameraControl.moveSpeed;
        cameraSpeedSlider.onValueChanged.AddListener(val => { cameraControl.SetPanSpeed(val); });

        // 시작할 땐 Settings 서브패널 닫아두기
        settingsPanel.SetActive(false);
        
        // ▶ BGM 볼륨 초기화 & 바인딩
        if (bgmSource != null && bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.value = bgmSource.volume;
            bgmVolumeSlider.onValueChanged.AddListener(v => bgmSource.volume = v);
        }
    }

    // Resume
    private void OnResume()
    {
        GameManager.Instance.TogglePause();
    }

    // Save
    private void OnSave()
    {
        // 웨이브가 진행 중이면 저장 불가
        if (WaveManager.Instance.IsWaveRunning)
        {
            NotificationService.Notify("Cannot Save during a wave!");
            GameManager.Instance.TogglePause();
            return;
        }
        
        DoSave();
        NotificationService.Notify("Game Saved!");
        GameManager.Instance.TogglePause(); // 저장 후 Resume 혹은 그냥 토글
    }
    
    // PausePanelController 내에…
    private void DoSave()
    {
        var saveData = new GameSaveData {
            gold   = ResourceManager.Instance.Gold,
            waveIndex   = WaveManager.Instance.CurrentWaveIndex,
            towers = BuildingSystem.Instance.GetAllTowerData()
        };
        SaveSystem.Save(saveData);
    }
    
    // Load (불러온 뒤 자동 Resume)
    private void OnLoad()
    {
        if (WaveManager.Instance.IsWaveRunning)
        {
            NotificationService.Notify("Cannot Load during a wave!");
            GameManager.Instance.TogglePause();
            return;
        }
        
        DoLoad();
        NotificationService.Notify("Game Loaded!");
        GameManager.Instance.TogglePause();
        
        if (SaveSystem.HasSave())
        {
            SaveSystem.QueueLoad();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else NotificationService.Notify("No save data.");
    }

    private void DoLoad()
    {
        GameSaveData data = SaveSystem.Load();
        if (data == null) return;

        // 1) 리셋
        BuildingSystem.Instance.ClearAllTowers();
        // … 유닛도 동일

        // 2) 값 복원
        ResourceManager.Instance.SetGold(data.gold);

        // ③ 저장된 타워 순회하며 재생성
        foreach (var ts in data.towers)
        {
            BuildingSystem.Instance.SpawnTowerFromSave(ts);
        }
    }
    
    // Settings 열기: Pause 닫고 Settings 열기
    private void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Settings 닫기: Settings 닫고 Pause 열기
    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    // Quit
    private void OnQuit()
    {
        GameManager.Instance.OnExitClick();
    }
}
