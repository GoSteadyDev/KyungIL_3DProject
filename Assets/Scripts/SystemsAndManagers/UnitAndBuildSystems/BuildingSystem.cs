using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private TowerDatabase towerDatabase;

    private List<ITower> allTowers = new List<ITower>();
    private BuildingPoint currentBuildPoint;
    
    public void Register(ITower t)   => allTowers.Add(t);
    public void Unregister(ITower t) => allTowers.Remove(t);
    
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 타입, 레벨, 분기 코드로 데이터+프리팹 Entry 조회
    /// </summary>
    public TowerDatabase.Entry GetTowerEntry(TowerType type, int level, string pathCode)
    {
        return towerDatabase.GetEntry(type, level, pathCode);
    }

    /// <summary>
    /// 타입, 레벨에 가능한 업그레이드 Entry 목록 조회
    /// </summary>
    public IEnumerable<TowerDatabase.Entry> GetUpgradeOptions(TowerType type, int level)
    {
        return towerDatabase.GetEntries(type, level);
    }
    
    /// <summary>
    /// 타일에서 빌드 시작 시 호출, 빌드 UI 표시
    /// </summary>
    public void OpenBuildUI(BuildingPoint point)
    {
        currentBuildPoint = point;
        UIManager.Instance.ShowTowerPanelByLevel(0, point.transform);
        
        var baseTowers = towerDatabase.entries
            .Where(e => e.data.level == 1 && string.IsNullOrEmpty(e.data.pathCode))
            .Select(e => e.data)
            .ToList();
        UIManager.Instance.UpdateTowerGuidePanelForCreation(baseTowers);
    }
    
    /// <summary>
    /// UI에서 타워 선택 시 호출
    /// </summary>
    public void OnTowerSelected(TowerData data)
    {
        if (currentBuildPoint != null)
        {
            bool success = TowerBuilder.Instance.BuildTower(data, currentBuildPoint.transform.position);
            if (success)
                Destroy(currentBuildPoint.gameObject);
            currentBuildPoint = null;
        }
        UIManager.Instance.HideAllTowerPanels();
    }

    /// <summary>
    /// 기존 타워를 업그레이드
    /// </summary>
    public void UpgradeTower(ITower oldTower, TowerData newData)
    {
        if (ResourceManager.Instance.TrySpendGold(newData.upgradeCost) == false)
            return;
        
        var oldMb = oldTower as MonoBehaviour;
        Unregister(oldTower);
        Destroy(oldMb.gameObject);

        var entry = GetTowerEntry(newData.towerType, newData.level, newData.pathCode);
        var go = TowerBuilder.Instance.BuildTower(newData, entry.prefab, oldMb.transform.position, oldMb.transform.rotation);
        Register(go);

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
    
    /// <summary>
    /// 게임 저장 정보를 바탕으로 타워 다시 생성
    /// </summary>
    public void SpawnTowerFromSave(TowerSaveData ts)
    {
        var entry = GetTowerEntry(ts.type, ts.level, ts.pathCode);
        if (entry.prefab == null) return;
        var tower = TowerBuilder.Instance.BuildTower(ts.type, ts.level, ts.pathCode, ts.pos);
        Register(tower);
    }

    /// <summary>
    /// 현재 씬에 있는 모든 타워 상태 수집
    /// </summary>
    public List<TowerSaveData> GetAllTowerData()
    {
        var list = new List<TowerSaveData>();
        
        foreach (var tower in allTowers.ToList())
        {
            var mb = tower as MonoBehaviour;

            // ① 파괴된 타워라면 리스트에서 제거하고 건너뛰기
            if (mb == null || mb.Equals(null))
            {
                allTowers.Remove(tower);
                continue;
            }
            
            var type = tower.GetTowerType();
            var lvl = tower.GetCurrentLevel();
            var trs = tower.GetTransform();
            
            list.Add(new TowerSaveData
            {
                type = type, level = lvl, pathCode = (tower as BaseTower).PathCode,
                pos = trs.position, rot = trs.rotation.eulerAngles
            });
        }
        return list;
    }
    
    /// <summary>
    /// 모든 타워 제거
    /// </summary>
    public void ClearAllTowers()
    {
        // 기존: foreach (var tower in allTowers)
        foreach (var tower in allTowers)
        {
            if (tower is MonoBehaviour mb && mb != null)  // ← mb != null 추가
            {
                Destroy(mb.gameObject);
            }
        }
        allTowers.Clear();
    }
}