using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{ 
    public static UIManager Instance;
    
    [Header("Warning Settings")]
    private Coroutine warningCoroutine;
    public TextMeshProUGUI warningText;
    [SerializeField] private float warningDuration = 2f;
    
    [Header("HpViewer Settings")]
    [SerializeField] private Canvas hpViewerCanvas;
    [SerializeField] private TextMeshProUGUI goldText;
    
    [Header("GameplayUI Settings")]
    [SerializeField] private TextMeshProUGUI GameoverText;
    [SerializeField] private Button GameoverCheckButton;
    [SerializeField] private GameObject pausePanel;
    
    [Header("WaveUI Settings")]
    [SerializeField] private TextMeshProUGUI WaveNumberText;

    [Header("BuildingUI Settings")]
    [SerializeField] private GameObject towerBuildUI;             // World Space UI Panel
    [SerializeField] private Canvas towerBuildCanvas;             // World Space Canvas (부모)
    
    private void Awake() 
    {
        Instance = this;
        HPViewerSpawner.Initialize(hpViewerCanvas.transform);
    }
    
    private void Start()
    {
        ResourceManager.Instance.OnGoldChanged += UpdateGoldText;
        UpdateGoldText(); // 초기값
    }

    private void UpdateGoldText()
    {
        goldText.text = "Gold : " + ResourceManager.Instance.Gold.ToString();
    }
    
    public void ShowWarning(string message)
    {
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(ShowWarningRoutine(message));
    }
    
    private IEnumerator ShowWarningRoutine(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(warningDuration);

        warningText.gameObject.SetActive(false);
    }
    
    public void ShowGameOver(string message)
    {
        GameoverText.gameObject.SetActive(true);
        GameoverText.text = message;

        GameoverCheckButton.gameObject.SetActive(true);
        GameoverCheckButton.interactable = true;
    }
    
    public void OnGameOverButtonClick()
    {
        GameManager.Instance.OnExitClick();
    }

    public void ShowPauseUI(bool isShow)
    {
        pausePanel.SetActive(isShow);
    }
    
    
    public void ShowWaveNumber(int waveNumber)
    {
        WaveNumberText.text = "Wave : " + waveNumber.ToString();
    }

    public void ShowBuildUI(Vector3 worldPos)
    {
        StartCoroutine(ShowWithDelay(worldPos));
    }

    private IEnumerator ShowWithDelay(Vector3 worldPos)
    {
        yield return new WaitForEndOfFrame();

        // ✅ 위치: BuildPoint 위로 띄우기
        towerBuildUI.transform.position = worldPos + new Vector3(0f, 15f, 0f);

        // ✅ 회전: 항상 카메라를 향하도록
        towerBuildUI.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        // ✅ Canvas 자식으로 붙이기 (안정성)
        towerBuildUI.transform.SetParent(towerBuildCanvas.transform);

        towerBuildUI.SetActive(true);
    }
    public void HideBuildUI()
    {
        towerBuildUI.SetActive(false);
    }
}