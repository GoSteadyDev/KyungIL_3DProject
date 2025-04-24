using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")]
    [SerializeField] private TowerType TowerType;
    [SerializeField] private int currentLevel = 0;
    
    [Header("Projectile Settings")]
    [SerializeField] private Archer archerUnit;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite icon;

    public string GetDisplayName() => "ArcherTower";
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => archerUnit.GetAttackRange();
    public Transform GetTransform() => transform;
    
    public TowerType GetTowerType() => TowerType;
    public int GetCurrentLevel() => currentLevel;
    
    private void Awake()
    {
        if (archerUnit == null)
            archerUnit = GetComponentInChildren<Archer>();
    }
}