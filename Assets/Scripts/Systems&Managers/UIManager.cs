using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    [SerializeField] private Canvas hpViewerCanvas;

    private void Awake() 
    {
        HPViewerSpawner.Initialize(hpViewerCanvas.transform);
    }
}


