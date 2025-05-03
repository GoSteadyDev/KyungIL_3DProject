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
        foreach (var t in allTowers)
        {
            var type = t.GetTowerType();
            var lvl = t.GetCurrentLevel();
            var trs = t.GetTransform();
            list.Add(new TowerSaveData
            {
                type = type,
                level = lvl,
                pathCode = (t as BaseTower).PathCode,
                pos = trs.position,
                rot = trs.rotation.eulerAngles
            });
        }
        return list;
    }
    
    /// <summary>
    /// 모든 타워 제거
    /// </summary>
    public void ClearAllTowers()
    {
        foreach (var t in allTowers)
            if (t is MonoBehaviour mb)
                Destroy(mb.gameObject);
        allTowers.Clear();
    }

}

// public class BuildingSystem : MonoBehaviour
// {
//     public static BuildingSystem Instance;
//
//     [SerializeField] private TowerDatabase towerDatabase;
//     [SerializeField] private List<TowerData> allTowerTemplates;
//     
//     private List<ITower> allTowers = new List<ITower>();
//     private BuildingPoint currentBuildPoint;
//     
//     public void Register(ITower t)   => allTowers.Add(t);
//     public void Unregister(ITower t) => allTowers.Remove(t);
//     
//     private void Awake()
//     {
//         Instance = this;
//     }
//     
//     // 저장된 데이터 한 건을 보고, 해당 타워를 다시 생성해서 리스트에 등록
//     public void SpawnTowerFromSave(TowerSaveData ts)
//     {
//         // 1) 어느 Template인지 찾아 온다 (type, level, pathCode)
//         //    분기 업그레이드가 필요 없다면 pathCode 무시하고 GetTemplate(type, level) 만 써도 됩니다.
//         TowerData data = GetTemplateByPath(ts.type, ts.level - 1, ts.pathCode)
//                                  ?? GetTemplate(ts.type, ts.level);
//         if (data == null)
//         {
//             return;
//         }
//
//         // 2) Instantiate
//         var go = Instantiate(data.towerPrefab, (Vector3)ts.pos,Quaternion.Euler(ts.rot));
//         // 3) ITower 컴포넌트로 등록
//         if (go.TryGetComponent<ITower>(out var towerComp))
//             allTowers.Add(towerComp);
//     }
//
//     public List<TowerSaveData> GetAllTowerData()
//     {
//         var list = new List<TowerSaveData>();
//         foreach (var t in allTowers)
//         {
//             var type  = t.GetTowerType();
//             var lvl   = t.GetCurrentLevel();
//             var trs   = t.GetTransform();
//             list.Add(new TowerSaveData {
//                 type     = type,
//                 level    = lvl,
//                 pos      = trs.position,
//                 rot      = trs.rotation.eulerAngles
//             });
//         }
//         return list;
//     }
//     
//     public void ClearAll()
//     {
//         foreach (var t in allTowers)
//             if (t is MonoBehaviour mb) Destroy(mb.gameObject);
//         allTowers.Clear();
//     }
//     
//     public TowerData GetTemplate(TowerType type, int level)
//     {
//         return allTowerTemplates.FirstOrDefault(t => t.towerType == type && t.level == level);
//     }
//     
//     public TowerData GetNextTemplate(TowerType type, int currentLevel)
//     {
//         return allTowerTemplates.FirstOrDefault(t => t.towerType == type && t.level == currentLevel + 1);
//     }
//     
//     public TowerData GetTemplateByPath(TowerType type, int currentLevel, string pathCode)
//     {
//         return allTowerTemplates.FirstOrDefault(t =>
//             t.towerType == type &&
//             t.level > currentLevel &&
//             t.pathCode == pathCode);
//     }
//     
//     public void OpenBuildUI(BuildingPoint point)
//     {
//         currentBuildPoint = point;
//         UIManager.Instance.ShowTowerPanelByLevel(0, point.transform.position);
//     }
//     
//     public void OnTowerSelected(TowerData data)
//     {
//         if (currentBuildPoint != null)
//         {
//             bool success = TowerBuilder.Instance.BuildTower(data, currentBuildPoint.transform.position);
//
//             if (success)
//             {
//                 Destroy(currentBuildPoint.gameObject);
//                 currentBuildPoint = null;
//             }
//         }
//
//         UIManager.Instance.HideAllTowerPanels();
//     }
//     
//     // 순차 업그레이드 메서드
//     public void UpgradeNextTower(ITower tower)
//     {
//         var type = tower.GetTowerType();
//         var currentLevel = tower.GetCurrentLevel();
//         var nextTemplate = GetNextTemplate(type, currentLevel);
//
//         if (nextTemplate == null)
//         {
//             return;
//         }
//
//         // if (ResourceManager.Instance.TrySpendGold(nextTemplate.cost) == false)
//         // {
//         //     return;
//         // }
//
//         var pos = tower.GetTransform().position;
//         var rot = tower.GetTransform().rotation;
//
//         TowerBuilder.Instance.UpgradeTower(tower, nextTemplate.towerPrefab, pos, rot);
//         UIManager.Instance.HideAllTowerPanels();
//     }
//     
//     // 선택 (분기형) 업그레이드 메서드
//     public void UpgradeWithTemplate(ITower tower, TowerData selectedData)
//     {
//         // if (ResourceManager.Instance.TrySpendGold(selectedData.cost) == false)
//         // {
//         //     return;
//         // }
//
//         var pos = tower.GetTransform().position;
//         var rot = tower.GetTransform().rotation;
//
//         TowerBuilder.Instance.UpgradeTower(tower, selectedData.towerPrefab, pos, rot);
//         UIManager.Instance.HideAllTowerPanels();
//     }
//     

// }
