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
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float warningDuration = 2f;
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
    [SerializeField] private int waveStartGoldThreshold = 4;
    private bool waveStartHintShown = false;
    
    [Header("ObjectInfoUI Settings")]
    [SerializeField] private GameObject objectInfoPanelRoot;
    [SerializeField] private GameObject barracksPanelRoot;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image iconImage;
    
    [Header("TowerUI Settings")]
    [SerializeField] private Canvas towerBuildCanvas;             // World Space Canvas (부모)
    [SerializeField] private GameObject towerInfoPanelRoot;
    [SerializeField] private GameObject towerCreatePanel; // Lv0 (빌딩 포인트 클릭 시)
    [SerializeField] private List<GameObject> towerLevelPanels; // Lv1~Lv3
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI towerDescText;
    [SerializeField] private Image towerIconImage;
    
    private Dictionary<int, GameObject> towerPanelDict;
    Camera cam;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 이미 다른 UIManager 가 있다면 이놈은 지워버린다!
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        NotificationService.OnNotify += ShowWarning;
        
        HPViewerSpawner.Initialize(followUIControlCanvas.transform);
        
        towerPanelDict = new Dictionary<int, GameObject>();
    
        // Lv1~Lv3 등록
        for (int i = 0; i < towerLevelPanels.Count; i++)
            towerPanelDict[i + 1] = towerLevelPanels[i];
        
        cam = Camera.main;
    }
    
    private void OnDestroy()
    {
        NotificationService.OnNotify -= ShowWarning;
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnGoldChanged -= EvaluateWaveStartHint;
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
        
        if (waveNotRunning && noSpendableGold && waveStartHintShown == false)
        {
            ShowWarning("Start Wave When You Are Ready!");
            waveStartHintShown = true;
        }
        
        // 조건이 풀리면, 다음에 또 알릴 수 있도록 리셋
        else if ((waveNotRunning == false || noSpendableGold == false) && waveStartHintShown)
        { 
            waveStartHintShown = false;
        }
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
        WaveDescriptionText.text = $"Wave {waveNumber} Enemies {totalEnemies - killedEnemies} / {totalEnemies}";
    }
    
    /// <summary>
    /// level에 맞는 패널을 target 위에 띄웁니다.
    /// </summary>
    public void ShowTowerPanelByLevel(int level, Transform target)
    {
        // 레벨별로 원하는 Y 오프셋을 정해 둡니다.
        // level 0 → 건설용, level 1~3 → 업그레이드용
        float yOffset = (level == 0) ? 16f : 18f;
        Vector3 worldOffset = Vector3.up * yOffset;

        // panel 선택
        GameObject panelGO;
        if (level == 0)
        {
            panelGO = towerCreatePanel;
        }
        else if (!towerPanelDict.TryGetValue(level, out panelGO))
        {
            Debug.LogWarning($"Level {level} 패널이 없습니다!");
            return;
        }

        // 코루틴으로 패널 초기화 + 활성화
        StartCoroutine(ShowPanelWithDelay(panelGO, target, worldOffset));
    }
    
    public IEnumerator ShowPanelWithDelay(GameObject panelGO, Transform target, Vector3 worldOffset)
    {
        yield return new WaitForEndOfFrame();

        var panel = panelGO.GetComponent<FollowUIPanel>();
        if (panel == null) yield break;

        panel.Initialize(target, worldOffset);
    }
    
    public void ShowTowerInfoPanel(IHasInfoPanel targetData, Transform towerTransform)
    {
        // 기존 패널들 숨기기
        HideAllBuildingPanels();

        if (targetData == null || towerTransform == null) 
            return;

        // 1) 패널 내용 세팅
        towerNameText.text  = targetData.GetDisplayName();
        towerDescText.text  = targetData.GetDescription();

        // 2) FollowUIPanel 가져오기
        var follow = towerInfoPanelRoot.GetComponent<FollowUIPanel>();
        if (follow == null)
        {
            Debug.LogError("towerInfoPanelRoot에 FollowUIPanel이 없습니다!");
            return;
        }

        // // 3) 원하는 오프셋 계산 (타워 오른쪽 + 살짝 위)
        Vector3 worldOffset = Vector3.right * 30f   // 오른쪽으로 2유닛
                              + Vector3.down * 20f;     // 위로 1유닛
        
        // 4) 초기화 (Activate + 위치 갱신 + 따라다니기 시작)
        follow.Initialize(towerTransform, worldOffset);
    }

    
    public void ShowUnitInfoPanel(IHasInfoPanel target)
    {
        HideAllBuildingPanels(); // 모든 Info 패널 숨김

        if (target == null) return;

        objectInfoPanelRoot.SetActive(true);
        nameText.text = target.GetDisplayName();
        descText.text = target.GetDescription();
        iconImage.sprite = target.GetIcon();
    }

    public void ShowBarrackPanel()
    {
        barracksPanelRoot.SetActive(true);
    }
    
    public void HideAllTowerPanels()
    {
        // Lv0 타워 건설 패널 닫기
        if (towerCreatePanel != null)
        {
            var createPanel = towerCreatePanel.GetComponent<FollowUIPanel>();
            if (createPanel != null)
                createPanel.Close();
        }
        // Lv1~Lv3 업그레이드 패널 닫기
        if (towerPanelDict != null)
        {
            foreach (var kv in towerPanelDict)
            {
                GameObject panelGO = kv.Value;
                if (panelGO != null)
                {
                    var panel = panelGO.GetComponent<FollowUIPanel>();
                    if (panel != null)
                        panel.Close();
                }
            }
        }
        // 정보 패널 닫기
        HideInfoPanel();
    }
    
    public void HideInfoPanel()
    {
        attackRangeViewer.gameObject.SetActive(false);
        HideAllBuildingPanels();
    }
    private void HideAllBuildingPanels()
    {
        towerInfoPanelRoot.SetActive(false);
        objectInfoPanelRoot.SetActive(false);
        barracksPanelRoot.SetActive(false);
    }
}