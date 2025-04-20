using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;
    
    private BuildingPoint currentBuildPoint;
    
    private void Awake()
    {
        Instance = this;
    }

    public void OpenBuildUI(BuildingPoint point)
    {
        currentBuildPoint = point;
        UIManager.Instance.ShowBuildUI(point.transform.position);
    }

    public void OnTowerSelected(GameObject towerPrefab)
    {
        if (currentBuildPoint != null)
        {
            bool success = TowerBuilder.Instance.BuildTower(towerPrefab, currentBuildPoint.transform.position);

            if (success)
            {
                Destroy(currentBuildPoint.gameObject);
                currentBuildPoint = null;
            }
        }

        UIManager.Instance.HideBuildUI();
    }
}
