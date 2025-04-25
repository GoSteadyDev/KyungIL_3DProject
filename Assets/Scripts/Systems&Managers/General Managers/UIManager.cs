using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public static class NotificationService
{
    // 메시지를 받을 구독자(UIManager 등)
    public static event Action<string> OnNotify;

    // 어디서든 이 메서드 한 줄만 호출하면 구독자들에게 메시지가 전달됩니다
    public static void Notify(string message)
    {
        OnNotify?.Invoke(message);
    }
}

public class UIManager : MonoBehaviour
{ 
    public static UIManager Instance;
    
    [Header("Warning Settings")]
    [SerializeField] private float warningDuration = 2f;
    public TextMeshProUGUI warningText;
    private Coroutine warningCoroutine;
    
    [Header("FollowUI Settings")]
    [SerializeField] private Canvas followUIControlCanvas;
    [SerializeField] private GameObject attackRangeViewer;
    
    [Header("GameplayUI Settings")]
    [SerializeField] private TextMeshProUGUI GameoverText;
    [SerializeField] private Button GameoverCheckButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject pausePanel;
    
    [Header("WaveUI Settings")]
    [SerializeField] private TextMeshProUGUI WaveDescriptionText;
    
    [Header("Wave Hint Settings")]
    [SerializeField] private int waveStartGoldThreshold = 4;
    private bool waveStartHintShown = false;

    [Header("TowerUI Settings")]
    [SerializeField] private Canvas towerBuildCanvas;             // World Space Canvas (부모)
    [SerializeField] private GameObject towerCreatePanel; // Lv0 (빌딩 포인트 클릭 시)
    [SerializeField] private List<GameObject> towerLevelPanels; // Lv1~Lv3
    private Dictionary<int, GameObject> towerPanelDict;

    [Header("InfoPanel Settings")]
    [SerializeField] private GameObject infoPanelRoot;
    
    [SerializeField] private GameObject barracksPanelRoot;
    [SerializeField] private GameObject upgradePanelRoot;
    [SerializeField] private GameObject storePanelRoot;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image iconImage;

    private void Awake()
    {
        Instance = this;
        NotificationService.OnNotify += ShowWarning;
        
        HPViewerSpawner.Initialize(followUIControlCanvas.transform);
        
        towerPanelDict = new Dictionary<int, GameObject>();
    
        // Lv1~Lv3 등록
        for (int i = 0; i < towerLevelPanels.Count; i++)
            towerPanelDict[i + 1] = towerLevelPanels[i];
    }
    
    private void Start()
    {
        ResourceManager.Instance.OnGoldChanged += UpdateGoldText;
        ResourceManager.Instance.OnGoldChanged += EvaluateWaveStartHint;
        UpdateGoldText();        // 초기 골드 표시
        EvaluateWaveStartHint(); // 게임 시작 시에도 한 번 체크
    }
    
    // 돈 부족하면 웨이브 시작하도록 알림 메시지
    private void EvaluateWaveStartHint()
    {
        bool waveNotRunning   = !WaveManager.Instance.IsWaveRunning;
        bool noSpendableGold  = ResourceManager.Instance.Gold <= waveStartGoldThreshold;
        
        if (waveNotRunning && noSpendableGold && !waveStartHintShown)
        {
            ShowWarning("Start Wave When You Are Ready!");
            waveStartHintShown = true;
        }
        
        // 조건이 풀리면, 다음에 또 알릴 수 있도록 리셋
        else if ((!waveNotRunning || !noSpendableGold) && waveStartHintShown)
        { 
            waveStartHintShown = false;
        }
    }
    
    private void OnDestroy()
    {
        NotificationService.OnNotify -= ShowWarning;
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnGoldChanged -= EvaluateWaveStartHint;
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
    
    public void ShowWaveInfo(int waveNumber, int killedEnemies, int totalEnemies)
    {
        WaveDescriptionText.text = $"Wave {waveNumber}\n Enemies {totalEnemies - killedEnemies} / {totalEnemies}";
    }
    
    public void ShowTowerPanelByLevel(int level, Vector3 worldPos)
    {
        if (level == 0)
        {
            ShowTowerCreatePanel(worldPos);
        }
        else if (towerPanelDict.TryGetValue(level, out var panel))
        {
            StartCoroutine(ShowPanelWithDelay(panel, worldPos));
        }
    }

    private void ShowTowerCreatePanel(Vector3 worldPos)
    {
        StartCoroutine(ShowPanelWithDelay(towerCreatePanel, worldPos));
    }

    private IEnumerator ShowPanelWithDelay(GameObject panel, Vector3 worldPos)
    {
        yield return new WaitForEndOfFrame();

        panel.transform.position = worldPos + new Vector3(0f, 15f, 0f);
        panel.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        panel.transform.SetParent(towerBuildCanvas.transform);
        panel.SetActive(true);
    }
    
    public void HideAllTowerPanels()
    {
        towerCreatePanel.SetActive(false);

        foreach (var panel in towerPanelDict.Values)
            panel.SetActive(false);
        
        HideInfoPanel();
    }
    
    public void ShowInfoPanel(IHasInfoPanel target)
    {
        HideAllBuildingPanels(); // 우선 모든 패널 비활성화
    
        if (target == null) return;
    
        // ➤ Barrack
        if (target is BarrackController)
        {
            barracksPanelRoot.SetActive(true);
        }
        // ➤ Upgrade Center
        else if (target is UpgradeCenterController)
        {
            upgradePanelRoot.SetActive(true);
        }
        // ➤ Store
        else if (target is StoreController)
        {
            storePanelRoot.SetActive(true);
        }
        // ➤ 기본 Info Panel
        else
        {
            infoPanelRoot.SetActive(true);
            nameText.text = target.GetDisplayName();
            descText.text = target.GetDescription();
            iconImage.sprite = target.GetIcon();
        }
    }
    
    public void HideInfoPanel()
    {
        attackRangeViewer.gameObject.SetActive(false);
        HideAllBuildingPanels();
    }

    public void ShowBarrackPanel()
    {
        barracksPanelRoot.SetActive(true);
    }

    private void HideAllBuildingPanels()
    {
        infoPanelRoot.SetActive(false);
        barracksPanelRoot.SetActive(false);
        upgradePanelRoot.SetActive(false);
        storePanelRoot.SetActive(false);
    }
}