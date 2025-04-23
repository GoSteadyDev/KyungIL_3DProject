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
        UpgradeTower(tower, 1);
    }
    
    public void UpgradeTower(ITower current, int nextIndex)
    {
        var template = current.GetTowerTemplate();
        Debug.Log($"[업그레이드] Template: {template}");

        if (template == null)
        {
            Debug.LogError("[업그레이드] template is NULL");
            return;
        }

        Debug.Log($"[업그레이드] Upgrade Count: {template.upgrades?.Count}");

        if (template.upgrades == null || template.upgrades.Count <= nextIndex)
        {
            Debug.LogError("[업그레이드] template.upgrades is NULL or too short");
            return;
        }
    }
    
    public void OnUpgradeAClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower != null)
        {
            BuildingSystem.Instance.UpgradeTower(tower, 2); // Lv3A index
            UIManager.Instance.HideTowerLv2Panel();
        }
    }

    public void OnUpgradeBClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower != null)
        {
            BuildingSystem.Instance.UpgradeTower(tower, 3); // Lv3B index
            UIManager.Instance.HideTowerLv2Panel();
        }
    }
    
    public void OnSellClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        TowerTemplate template = tower.GetTowerTemplate();
        int level = tower.GetCurrentLevel();

        if (template == null || template.upgrades.Count <= level)
        {
            Debug.LogWarning("타워 템플릿 정보가 잘못되었습니다.");
            return;
        }

        int cost = template.upgrades[level].cost;
        int refund = Mathf.RoundToInt(cost * 0.5f); // 또는 cost / 2

        ResourceManager.Instance.AddGold(refund);
        Debug.Log($"타워 판매: {cost} → 환급 골드 {refund}");

        // 타워 삭제
        GameObject towerGO = (tower as MonoBehaviour)?.gameObject;
        if (towerGO != null)
            GameObject.Destroy(towerGO);

        // UI 닫기
        UIManager.Instance.HideTowerLv1Panel();
        UIManager.Instance.HideTowerLv2Panel();
        UIManager.Instance.HideTowerLv3Panel();
    }
    
    public void OnCloseButtonClicked()
    {
        BuildingSystem.Instance.CancelBuild(); // 💥 빌딩 포인트까지 제거
    }
}