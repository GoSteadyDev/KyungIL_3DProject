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

    public bool BuildNewTower(TowerData data, Vector3 position)
    {
        if (ResourceManager.Instance.TrySpendGold(data.buildCost) == false)
            return false;
        
        // 데이터 기반 Entry 조회
        var entry = BuildingSystem.Instance.GetTowerEntry(data.towerType, data.level, data.pathCode);
        
        var tower = Instantiate(entry.prefab, position, Quaternion.identity);
        
        var towerComponent = tower.GetComponent<BaseTower>();
        towerComponent.Initialize(entry.data);
        BuildingSystem.Instance.Register(towerComponent);
        
        return true;
    }

    public BaseTower BuildUpgradTower(TowerData data, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var tower = Instantiate(prefab, position, rotation);
        
        var towerComponent = tower.GetComponent<BaseTower>();
        
        towerComponent.Initialize(data);
        BuildingSystem.Instance.Register(towerComponent);
        
        return towerComponent;
    }

    public BaseTower BuildTowerFromSave(TowerType type, int level, string pathCode, Vector3 position)
    {
        var entry = BuildingSystem.Instance.GetTowerEntry(type, level, pathCode);
        
        return BuildUpgradTower(entry.data, entry.prefab, position, Quaternion.identity);
    }
}