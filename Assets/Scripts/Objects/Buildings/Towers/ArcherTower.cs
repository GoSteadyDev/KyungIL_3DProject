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

    [Header("InfoPanel")]
    [SerializeField] private string displayName;
    [SerializeField] private string displayLevel;
    [SerializeField] private float displayAttackSpeed;
    [SerializeField] private float displayDamage;
    [SerializeField] private float displayRange;

    public Sprite GetIcon() => icon;
    public string GetDisplayName() => displayName;
    public string GetDescription() 
        => $"Tower Level : {displayLevel} \nDamage : {displayDamage} \nAttackSpeed : {displayAttackSpeed}\nAttackRange : {displayRange}";
    
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