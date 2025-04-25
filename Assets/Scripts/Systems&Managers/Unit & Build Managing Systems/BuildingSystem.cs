using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance;

    [SerializeField] private List<TowerTemplate> allTowerTemplates;
    
    private List<ITower> allTowers = new List<ITower>();
    private BuildingPoint currentBuildPoint;
    
    public void Register(ITower t)   => allTowers.Add(t);
    public void Unregister(ITower t) => allTowers.Remove(t);
    
    private void Awake()
    {
        Instance = this;
    }
    
    // 저장된 데이터 한 건을 보고, 해당 타워를 다시 생성해서 리스트에 등록
    public void SpawnTowerFromSave(TowerSaveData ts)
    {
        // 1) 어느 Template인지 찾아 온다 (type, level, pathCode)
        //    분기 업그레이드가 필요 없다면 pathCode 무시하고 GetTemplate(type, level) 만 써도 됩니다.
        TowerTemplate template = GetTemplateByPath(ts.type, ts.level - 1, ts.pathCode)
                                 ?? GetTemplate(ts.type, ts.level);
        if (template == null)
        {
            Debug.LogError($"저장 복원 실패: Template 없음 {ts.type} Lv{ts.level} / path {ts.pathCode}");
            return;
        }

        // 2) Instantiate
        var go = Instantiate(template.towerPrefab,
            (Vector3)ts.pos,
            Quaternion.Euler(ts.rot));
        // 3) ITower 컴포넌트로 등록
        if (go.TryGetComponent<ITower>(out var towerComp))
            allTowers.Add(towerComp);
    }

    public List<TowerSaveData> GetAllTowerData()
    {
        var list = new List<TowerSaveData>();
        foreach (var t in allTowers)
        {
            var type  = t.GetTowerType();
            var lvl   = t.GetCurrentLevel();
            var trs   = t.GetTransform();
            list.Add(new TowerSaveData {
                type     = type,
                level    = lvl,
                pos      = trs.position,
                rot      = trs.rotation.eulerAngles
            });
        }
        return list;
    }
    
    public void ClearAll()
    {
        foreach (var t in allTowers)
            if (t is MonoBehaviour mb) Destroy(mb.gameObject);
        allTowers.Clear();
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
