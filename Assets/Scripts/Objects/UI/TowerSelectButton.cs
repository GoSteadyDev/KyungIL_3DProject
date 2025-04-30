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

    public void OnUpgradeClicked()
    {
        // Sequential upgrade to next level
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        var type = tower.GetTowerType();
        var level = tower.GetCurrentLevel();
        // Get upgrade options and find next level
        var options = BuildingSystem.Instance
            .GetUpgradeOptions(type, level + 1)
            .Where(e => e.data.level == level + 1)
            .ToList();
        
        if (options.Count == 0)
        {
            Debug.LogWarning("No further sequential upgrade available.");
            return;
        }
        
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
        
        if (entry.prefab == null)
        {
            Debug.LogWarning($"No upgrade found for pathCode '{pathCode}'.");
            return;
        }
        
        BuildingSystem.Instance.UpgradeTower(tower, entry.data);
    }

    public void OnSellClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

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
