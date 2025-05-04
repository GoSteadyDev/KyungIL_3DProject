using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
public class TowerBuilder : MonoBehaviour
{
    public static TowerBuilder Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private TileDetector tileDetector;
    [SerializeField] private GameObject buildPointPrefab;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            var tile = tileDetector.GetTileUnderMouse();
            if (tile != null && !tile.IsUsed)
                OnTileClicked(tile);
        }
    }

    public void OnTileClicked(BuildingPointTile tile)
    {
        Instantiate(buildPointPrefab, tile.transform.position, Quaternion.identity);
        tile.PickedByPlayer();
    }

    public bool BuildTower(TowerData data, Vector3 position)
    {
        if (ResourceManager.Instance.TrySpendGold(data.buildCost) == false)
            return false;
        
        // 데이터 기반 Entry 조회
        var entry = BuildingSystem.Instance.GetTowerEntry(data.towerType, data.level, data.pathCode);
        
        var towerGO = Instantiate(entry.prefab, position, Quaternion.identity);
        
        var towerComp = towerGO.GetComponent<BaseTower>();
        towerComp.Initialize(entry.data);
        BuildingSystem.Instance.Register(towerComp);
        
        return true;
    }

    public BaseTower BuildTower(TowerData data, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var towerGO = Instantiate(prefab, position, rotation);
        
        var tower = towerGO.GetComponent<BaseTower>();
        
        tower.Initialize(data);
        BuildingSystem.Instance.Register(tower);
        
        return tower;
    }

    public BaseTower BuildTower(TowerType type, int level, string pathCode, Vector3 position)
    {
        var entry = BuildingSystem.Instance.GetTowerEntry(type, level, pathCode);
        
        return BuildTower(entry.data, entry.prefab, position, Quaternion.identity);
    }

    public BaseTower UpgradeTower(ITower oldTower, TowerData newData)
    {
        var oldMb = oldTower as MonoBehaviour;
        BuildingSystem.Instance.Unregister(oldTower);
        
        Destroy(oldMb.gameObject);
        
        var entry = BuildingSystem.Instance.GetTowerEntry(newData.towerType, newData.level, newData.pathCode);
        var tower = BuildTower(entry.data, entry.prefab, oldMb.transform.position, oldMb.transform.rotation);
        
        return tower;
    }
}