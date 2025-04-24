using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Template/TowerTemplate")]
public class TowerTemplate : ScriptableObject
{
    public List<UpgradeData> upgrades;
    
    [System.Serializable]
    public class UpgradeData
    {
        public GameObject towerPrefab;
        public int cost;
    }
}