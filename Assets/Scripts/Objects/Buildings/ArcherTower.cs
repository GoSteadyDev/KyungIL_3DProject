using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour, ISelectable, IHasInfoPanel, ITower
{
    [Header("Components")]
    [SerializeField] private Archer archerUnit;
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private int currentLevel = 0;
    
    [Header("UI Info")]
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