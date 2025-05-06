using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerSelectButton : MonoBehaviour
{
    [SerializeField] private TowerData towerData; // For building new tower
    [SerializeField] private string pathCode;      // For branch upgrade (e.g., "A", "B")

    public void OnTowerClicked()
    {
        // Build new tower at the selected point
        BuildingSystem.Instance.OnTowerSelected(towerData);
    }

    // 이렇게 순차처리와 분기처리를 구분해놓은 이유는, Lv3B -> Lv4B 처리도 생각
    // 즉 모든 걸 PathCode처리로 해놓을 수도 있었는데, 코드 명확화 + 순차만 가능한 업그레이드 타워도 염두에 두고 작성함
    public void OnUpgradeClicked()
    {
        // Sequential upgrade to next level
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        var type = tower.GetTowerType();
        var level = tower.GetCurrentLevel();
        
        // Get upgrade options and find next level
        var options = BuildingSystem.Instance.GetUpgradeOptions(type, level + 1);

        if (options.Count == 0) return;
            
        var nextData = options[0].data;
        BuildingSystem.Instance.UpgradeTower(tower, nextData);
    }

    public void OnUpgradeByPath()
    {
        // Branch upgrade based on pathCode
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        var type = tower.GetTowerType();
        var level = tower.GetCurrentLevel();
        // Retrieve entry for next level and pathCode
        
        var entry = BuildingSystem.Instance.GetTowerEntry(type, level + 1, pathCode);
        
        if (entry.prefab == null) return;
        
        BuildingSystem.Instance.UpgradeTower(tower, entry.data);
    }

    public void OnSellClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower as BaseTower;
        if (tower == null) return;

        // 데이터를 BuildingSystem에서 조회
        var entry = BuildingSystem.Instance.GetTowerEntry(tower.GetTowerType(),tower.GetCurrentLevel(),tower.PathCode);

        if (entry.data != null)
        {
            ResourceManager.Instance.AddGold(entry.data.sellPrice);
        }

        var towerGO = (tower as MonoBehaviour)?.gameObject;
        
        if (towerGO != null)
        {
            BuildingSystem.Instance.Unregister(tower);
            Destroy(towerGO);
        }

        UIManager.Instance.HideAllTowerPanels();
    }


    public void OnCloseButtonClicked()
    {
        BuildingSystem.Instance.CancelBuild();
    }
}
