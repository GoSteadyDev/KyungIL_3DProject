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
        BuildingSystem.Instance.CancelBuild(); // 💥 빌딩 포인트까지 제거
    }

}