using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;
    
    [SerializeField] private GameObject buildUI;
    
    private BuildingPoint currentBuildPoint;
    
    private void Awake()
    {
        Instance = this;
        buildUI.SetActive(false);
    }

    public void OpenBuildUI(BuildingPoint point)
    {
        currentBuildPoint = point;
        buildUI.SetActive(true);
    }

    public void OnTowerSelected(GameObject towerPrefab)
    {
        if (currentBuildPoint != null)
        {
            TowerBuilder.Instance.BuildTower(towerPrefab, currentBuildPoint.transform.position);
            Destroy(currentBuildPoint.gameObject);
            currentBuildPoint = null;
        }

        buildUI.SetActive(false);
    }
}
