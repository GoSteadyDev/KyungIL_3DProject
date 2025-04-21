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

    public void UpgradeTower(ITower tower, int index)
    {
        TowerTemplate template = tower.GetTowerTemplate();

        if (template == null || template.upgrades.Count <= index)
        {
            Debug.LogWarning("Upgrade info not found!");
            return;
        }

        TowerTemplate.UpgradeData data = template.upgrades[index];

        // 골드 체크는 TowerBuilder에서 할 수도 있고 여기서 해도 OK
        if (!ResourceManager.Instance.TrySpendGold(data.cost))
        {
            UIManager.Instance.ShowWarning("Not enough gold!");
            return;
        }

        // TowerBuilder에 넘김
        Vector3 pos = tower.GetTransform().position;
        Quaternion rot = tower.GetTransform().rotation;

        TowerBuilder.Instance.UpgradeTower(tower, data.towerPrefab, pos, rot);

        UIManager.Instance.HideTowerLv1Panel();
    }

    
    public void CancelBuild()
    {
        if (currentBuildPoint != null)
        {
            Destroy(currentBuildPoint.gameObject); // 💥 BuildPoint 제거
            currentBuildPoint = null;
            UIManager.Instance.HideBuildUI();
            // 타일 복구 ❌ X → 다시 생성 방지
        }
    }
}
