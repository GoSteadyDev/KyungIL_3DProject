using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour, ITower, IHasRangeUI, IHasInfoPanel
{     
    protected TowerData data { get; private set; }

    [Header("Attack Origin")]
    [SerializeField] protected Transform firePoint;

    private float attackTimer;

    public TowerType GetTowerType() => data.towerType;
    public int GetCurrentLevel()   => data.level;
    public Transform GetTransform() => transform;
    
    // ─── IHasRangeUI 구현 ─────────────────────────────────
    public virtual float GetAttackRange() => data.attackRange;
    Transform IHasRangeUI.GetTransform() => transform;
        
    // ─── IHasInfoPanel 구현 ────────────────────────────────
    public virtual Sprite GetIcon()        => data.icon;
    public virtual string GetDisplayName() => data.displayName;
    public virtual string GetDescription() => data.description;
    
    protected abstract Transform FindTarget();
    protected abstract void Attack(Transform target);

    public void Initialize(TowerData newData)
    {
        data = newData;
        RefreshStats();
        RefreshVisuals();
    }

    protected virtual void RefreshStats() { }
    protected virtual void RefreshVisuals() { }

    protected virtual void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f / data.attackSpeed)
        {
            var target = FindTarget();
            if (target != null)
                Attack(target);
            
            attackTimer = 0f;
        }
    }
    public virtual void Upgrade(TowerData nextData)
    {
        Initialize(nextData);
    }
}