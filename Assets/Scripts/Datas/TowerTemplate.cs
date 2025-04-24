using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Template/TowerTemplate")]
public class TowerTemplate : ScriptableObject
{
    public TowerType towerType;         // 예: Cannon
    public int level;                   // 예: 1, 2, 3
    public int cost;                    // 업그레이드 비용
    public string pathCode;
    public GameObject towerPrefab;      // 해당 레벨의 프리팹
}
