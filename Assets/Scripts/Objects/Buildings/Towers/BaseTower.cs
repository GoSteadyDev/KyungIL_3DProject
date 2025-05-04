using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower : MonoBehaviour, ITower, IHasRangeUI, IHasInfoPanel
{     
    protected TowerData data { get; private set; }

    [Header("Attack Origin")]
    [SerializeField] protected Transform firePoint;

    private float attackTimer;

    public string PathCode => data.pathCode;
    // ─── ITower 구현 ─────────────────────────────────
    public TowerType GetTowerType() => data.towerType;
    public int GetCurrentLevel()   => data.level;
    public Transform GetTransform() => transform;
    
    // ─── IHasRangeUI 구현 ─────────────────────────────────
    public virtual float GetAttackRange() => data.attackRange;
        
    // ─── IHasInfoPanel 구현 ────────────────────────────────
    public virtual Sprite GetIcon()        => data.icon;
    public virtual string GetDisplayName() => data.displayName;
    public virtual string GetDescription()
    {
        return $"\n- Damage: {data.damage}" +
               $"\n- Range: {data.attackRange}" +
               $"\n- AttackSpeed: {data.attackSpeed:F2}";
    }
    
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
}