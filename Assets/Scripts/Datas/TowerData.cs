using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    Transform GetTransform(); // 또는 property Transform Transform { get; }
    TowerTemplate GetTowerTemplate();
    int GetCurrentLevel();
}

[CreateAssetMenu(menuName = "Template/TowerTemplate")]
public class TowerTemplate : ScriptableObject
{
    public List<UpgradeData> upgrades;
    
    [System.Serializable]
    public class UpgradeData
    {
        public string levelName;
        public GameObject towerPrefab;
        public int cost;
    }
}