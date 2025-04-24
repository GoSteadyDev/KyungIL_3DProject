using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelectButton : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate;

    public void OnTowerClicked()
    {
        BuildingSystem.Instance.OnTowerSelected(towerTemplate);
    }
    
    public void OnUpgradeNextClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        BuildingSystem.Instance.UpgradeNextTower(tower); // Lv2
    }
    
    public void OnUpgradeWithTemplateClicked()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower != null)
        {
            BuildingSystem.Instance.UpgradeWithTemplate(tower, this.towerTemplate);
        }
    }
    
    public void OnUpgradeA()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        var type = tower.GetTowerType();
        var level = tower.GetCurrentLevel();

        var template = BuildingSystem.Instance.GetNextTemplate(type, level); // level + 1
        BuildingSystem.Instance.UpgradeWithTemplate(tower, template);
    }

    public void OnUpgradeB()
    {
        var tower = ObjectSelector.Instance.CurrentSelectedTower;
        var type = tower.GetTowerType();
        var level = tower.GetCurrentLevel();

        var template = BuildingSystem.Instance.GetTemplateB(type, level); // level + 2
        BuildingSystem.Instance.UpgradeWithTemplate(tower, template);
    }

    
    public void OnSellClicked()
    {
        ITower tower = ObjectSelector.Instance.CurrentSelectedTower;
        if (tower == null) return;

        TowerType type = tower.GetTowerType();
        int level = tower.GetCurrentLevel();

        // TowerTemplate 찾아오기
        TowerTemplate template = BuildingSystem.Instance.GetTemplate(type, level);
        if (template == null)
        {
            Debug.LogWarning("타워 템플릿을 찾을 수 없습니다.");
            return;
        }

        int refund = Mathf.RoundToInt(template.cost * 0.5f);
        ResourceManager.Instance.AddGold(refund);

        GameObject towerGO = (tower as MonoBehaviour)?.gameObject;
        if (towerGO != null)
            GameObject.Destroy(towerGO);

        UIManager.Instance.HideAllTowerPanels();
    }
    
    public void OnCloseButtonClicked()
    {
        BuildingSystem.Instance.CancelBuild(); // 💥 빌딩 포인트까지 제거
    }
}