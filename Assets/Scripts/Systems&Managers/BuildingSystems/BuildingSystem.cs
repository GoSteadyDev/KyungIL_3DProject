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
            // TowerBuilder에서 생성하는 로직 가져오기
        }

        buildUI.SetActive(false);
    }
}
