using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;

    [SerializeField] private List<TowerTemplate> allTowerTemplates;
    
    private BuildingPoint currentBuildPoint;
    
    private void Awake()
    {
        Instance = this;
    }

    public TowerTemplate GetTemplate(TowerType type, int level)
    {
        return allTowerTemplates.FirstOrDefault(t => t.towerType == type && t.level == level);
    }
    
    public TowerTemplate GetNextTemplate(TowerType type, int currentLevel)
    {
        return allTowerTemplates.FirstOrDefault(t => t.towerType == type && t.level == currentLevel + 1);
    }
    
    public TowerTemplate GetTemplateByPath(TowerType type, int currentLevel, string pathCode)
    {
        return allTowerTemplates.FirstOrDefault(t =>
            t.towerType == type &&
            t.level > currentLevel &&
            t.pathCode == pathCode);
    }
    
    public void OpenBuildUI(BuildingPoint point)
    {
        currentBuildPoint = point;
        UIManager.Instance.ShowTowerPanelByLevel(0, point.transform.position);
    }
    
    public void OnTowerSelected(TowerTemplate template)
    {
        if (currentBuildPoint != null)
        {
            bool success = TowerBuilder.Instance.BuildTower(template, currentBuildPoint.transform.position);

            if (success)
            {
                Destroy(currentBuildPoint.gameObject);
                currentBuildPoint = null;
            }
        }

        UIManager.Instance.HideAllTowerPanels();
    }
    
    // 순차 업그레이드 메서드
    public void UpgradeNextTower(ITower tower)
    {
        var type = tower.GetTowerType();
        var currentLevel = tower.GetCurrentLevel();
        var nextTemplate = GetNextTemplate(type, currentLevel);

        if (nextTemplate == null)
        {
            return;
        }

        if (!ResourceManager.Instance.TrySpendGold(nextTemplate.cost))
        {
            return;
        }

        var pos = tower.GetTransform().position;
        var rot = tower.GetTransform().rotation;

        TowerBuilder.Instance.UpgradeTower(tower, nextTemplate.towerPrefab, pos, rot);
        UIManager.Instance.HideAllTowerPanels();
    }
    
    // 선택 (분기형) 업그레이드 메서드
    public void UpgradeWithTemplate(ITower tower, TowerTemplate selectedTemplate)
    {
        if (!ResourceManager.Instance.TrySpendGold(selectedTemplate.cost))
        {
            return;
        }

        var pos = tower.GetTransform().position;
        var rot = tower.GetTransform().rotation;

        TowerBuilder.Instance.UpgradeTower(tower, selectedTemplate.towerPrefab, pos, rot);
        UIManager.Instance.HideAllTowerPanels();
    }
    
    public void CancelBuild()
    {
        if (currentBuildPoint != null)
        {
            Destroy(currentBuildPoint.gameObject); // 💥 BuildPoint 제거
            currentBuildPoint = null;
            
            UIManager.Instance.HideAllTowerPanels();
        }
    }
}
