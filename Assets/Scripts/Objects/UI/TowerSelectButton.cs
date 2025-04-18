using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelectButton : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;

    public void OnTowerClicked()
    {
        BuildingSystem.Instance.OnTowerSelected(towerPrefab);
    }

    public void OnCloseButtonClicked()
    {
        UIManager.Instance.HideBuildUI();
    }
}