using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private TowerTemplate towerTemplate;
    
    [Header("Projectile Settings")]
    [SerializeField] private Archer archerUnit;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite icon;

    public string GetDisplayName() => "ArcherTower"; 
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => archerUnit.GetAttackRange();
    public Transform GetTransform() => transform;
    public TowerTemplate GetTowerTemplate() => towerTemplate;
    public int GetCurrentLevel() => currentLevel;
    private void Awake()
    {
        if (archerUnit == null)
            archerUnit = GetComponentInChildren<Archer>();
    }
}