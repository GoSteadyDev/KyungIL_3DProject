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

    // 타입, 레벨, 분기 코드로 데이터+프리팹 Entry 조회
    public TowerDatabase.Entry GetTowerEntry(TowerType type, int level, string pathCode)
    {
        return towerDatabase.GetEntry(type, level, pathCode);
    }
    
    // 타입, 레벨에 가능한 업그레이드 Entry 목록 조회
    public List<TowerDatabase.Entry> GetUpgradeOptions(TowerType type, int level)
    {
        return towerDatabase.GetEntryByLevel(type, level);
    }
    
    // 타일에서 빌드 시작 시 호출, 타워 가이드 패널 UI에 담길 내용을 표시하기 위해 필요한 메서드
    public void OpenBuildUI(BuildingPoint point)
    {
        currentBuildPoint = point;
        UIManager.Instance.ShowTowerPanelByLevel(0, point.transform);
        
        var baseTowers = towerDatabase.entries
            .Where(entry => entry.data.level == 1 && string.IsNullOrEmpty(entry.data.pathCode))
            .Select(entry => entry.data)
            .ToList();
        
        UIManager.Instance.UpdateTowerGuidePanelForCreation(baseTowers);
    }
    
    // UI에서 타워 선택 시 호출되어 실제 타워를 생성하는 메서드
    public void OnTowerSelected(TowerData data)
    {
        if (currentBuildPoint != null)
        {
            bool success = TowerBuilder.Instance.BuildNewTower(data, currentBuildPoint.transform.position);
            if (success) Destroy(currentBuildPoint.gameObject);
            currentBuildPoint = null;
        }
        UIManager.Instance.HideAllTowerPanels();
    }

    // 기존 타워를 업그레이드 하기 위한 메서드
    public void UpgradeTower(ITower oldTower, TowerData newData)
    {
        if (ResourceManager.Instance.TrySpendGold(newData.upgradeCost) == false) return;
        
        var oldMb = oldTower as MonoBehaviour;
        Unregister(oldTower);
        Destroy(oldMb.gameObject);

        var entry = GetTowerEntry(newData.towerType, newData.level, newData.pathCode);
        var newTower = TowerBuilder.Instance.BuildUpgradTower(newData, entry.prefab, oldMb.transform.position, oldMb.transform.rotation);
        Register(newTower);

        UIManager.Instance.HideAllTowerPanels();
    }
    
    public void CancelBuild()
    {
        if (currentBuildPoint != null)
        {
            Destroy(currentBuildPoint.gameObject); // BuildPoint 제거
            currentBuildPoint = null;
             
            UIManager.Instance.HideAllTowerPanels();
        }
    }
    
    // 로드 시 사용될 메서드. 게임 저장 정보를 바탕으로 타워 다시 생성
    public void SpawnTowerFromSave(TowerSaveData towerSaveData)
    {
        var entry = GetTowerEntry(towerSaveData.type, towerSaveData.level, towerSaveData.pathCode);
        if (entry.prefab == null) return;
        var tower = TowerBuilder.Instance.BuildTowerFromSave(towerSaveData.type, towerSaveData.level, towerSaveData.pathCode, towerSaveData.pos);
        Register(tower);
    }

    // 현재 씬에 있는 모든 타워 상태 수집
    public List<TowerSaveData> GetAllTowerData()
    {
        var list = new List<TowerSaveData>();
        // 원본 리스트(allTowers)가 반복 도중 변경되면 예외가 나기 때문에 ToList()
        // ToList() : IEnumerable(열거 가능한 컬렉션)을 실제 List로 복사해서 반환하는 메서드
        
        foreach (var tower in allTowers.ToList())
        {
            var mb = tower as MonoBehaviour;

            // 파괴된 타워라면 리스트에서 제거하고 건너뛰기
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
    
    // 모든 타워 제거
    public void ClearAllTowers()
    {
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