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
    [SerializeField] private TextMeshProUGUI WaveDescriptionText;

    [Header("TowerUI Settings")]
    [SerializeField] private Canvas towerBuildCanvas;             // World Space Canvas (부모)
    [SerializeField] private GameObject towerBuildUI;             // World Space UI Panel
    [SerializeField] private GameObject towerLv1Panel;
    [SerializeField] private GameObject towerLv2Panel;
    [SerializeField] private GameObject towerLv3Panel;

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
    
    public void ShowWaveInfo(int waveNumber, int killedEnemies, int totalEnemies)
    {
        WaveDescriptionText.text = $"Wave {waveNumber}\n Enemies {totalEnemies - killedEnemies} / {totalEnemies}";
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
    
    public void ShowTowerLv1Panel(Vector3 worldPos)
    {
        StartCoroutine(ShowTowerLv1PanelWithDelay(worldPos));
    }

    private IEnumerator ShowTowerLv1PanelWithDelay(Vector3 worldPos)
    {
        yield return new WaitForEndOfFrame();

        towerLv1Panel.transform.position = worldPos + new Vector3(0f, 15f, 0f);
        towerLv1Panel.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        towerLv1Panel.transform.SetParent(towerBuildCanvas.transform);

        towerLv1Panel.SetActive(true);
    }

    public void HideTowerLv1Panel()
    {
        towerLv1Panel.SetActive(false);
    }
    
    public void ShowTowerLv2Panel(Vector3 worldPos)
    {
        StartCoroutine(ShowTowerLv2PanelWithDelay(worldPos));
    }

    private IEnumerator ShowTowerLv2PanelWithDelay(Vector3 worldPos)
    {
        yield return new WaitForEndOfFrame();

        towerLv2Panel.transform.position = worldPos + new Vector3(0f, 15f, 0f);
        towerLv2Panel.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        towerLv2Panel.transform.SetParent(towerBuildCanvas.transform);

        towerLv2Panel.SetActive(true);
    }

    public void HideTowerLv2Panel()
    {
        towerLv2Panel.SetActive(false);
    }
    
    public void ShowTowerLv3Panel(Vector3 worldPos)
    {
        StartCoroutine(ShowTowerLv3PanelWithDelay(worldPos));
    }

    private IEnumerator ShowTowerLv3PanelWithDelay(Vector3 worldPos)
    {
        yield return new WaitForEndOfFrame();

        towerLv3Panel.transform.position = worldPos + new Vector3(0f, 15f, 0f);
        towerLv3Panel.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        towerLv3Panel.transform.SetParent(towerBuildCanvas.transform);

        towerLv3Panel.SetActive(true);
    }

    public void HideTowerLv3Panel()
    {
        towerLv3Panel.SetActive(false);
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