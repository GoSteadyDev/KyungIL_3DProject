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
    
    public void OnUpgradeClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        BuildingSystem.Instance.UpgradeTower(tower, 1); // Lv2
    }
    
    public void OnUpgradeAClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower != null)
        {
            BuildingSystem.Instance.UpgradeTower(tower, 2); // Lv3A index
        }
    }

    public void OnUpgradeBClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower != null)
        {
            BuildingSystem.Instance.UpgradeTower(tower, 3); // Lv3B index
        }
    }
    
    public void OnSellClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        TowerTemplate template = tower.GetTowerTemplate();
        int level = tower.GetCurrentLevel();
        int cost = template.upgrades[level].cost;
        int refund = Mathf.RoundToInt(cost * 0.5f); // 또는 cost / 2

        ResourceManager.Instance.AddGold(refund);

        // 타워 삭제
        GameObject towerGO = (tower as MonoBehaviour)?.gameObject;
        if (towerGO != null)
            GameObject.Destroy(towerGO);

        // UI 닫기
        UIManager.Instance.HideAllTowerPanels();
    }
    
    public void OnCloseButtonClicked()
    {
        BuildingSystem.Instance.CancelBuild(); // 💥 빌딩 포인트까지 제거
    }
}