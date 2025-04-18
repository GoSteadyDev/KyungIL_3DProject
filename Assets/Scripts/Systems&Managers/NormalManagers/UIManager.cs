using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{ 
    public static UIManager Instance;
    
    private Coroutine warningCoroutine;
    public TextMeshProUGUI warningText;
    [SerializeField] private float warningDuration = 2f;
    
    [SerializeField] private Canvas hpViewerCanvas;
    [SerializeField] private TextMeshProUGUI goldText;
    
    [SerializeField] private TextMeshProUGUI GameoverText;
    [SerializeField] private Button GameoverCheckButton;
    
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
    
    [SerializeField] private GameObject pausePanel;

    public void ShowPauseUI(bool isShow)
    {
        pausePanel.SetActive(isShow);
    }
}