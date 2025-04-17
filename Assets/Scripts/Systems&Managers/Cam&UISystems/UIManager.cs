using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    [SerializeField] private Canvas hpViewerCanvas;
    [SerializeField] private TextMeshProUGUI goldText;
    private void Awake() 
    {
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
}