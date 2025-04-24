using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTower : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float slowRate = 0.25f;
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private float attackRange = 7.5f;
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject slowEffectPrefab;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite icon;
    
    private int damagePerSec => Mathf.CeilToInt(damage / slowDuration);
    public string GetDisplayName() => "SlowTower"; 
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\n Slow AttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerTemplate GetTowerTemplate() => towerTemplate;
    public int GetCurrentLevel() => currentLevel;

    private Transform targetTransform;
    private Animator animator;
    
    private bool isFiring = false;
    private float currentCooldown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (targetTransform != null)
        {
           animator.SetTrigger("IsAttack");
           if (!isFiring) Fire();
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    private void Fire()
    {
        if (targetTransform == null || isFiring) return;

        isFiring = true;

        EnemyController enemy = targetTransform.GetComponent<EnemyController>();
        if (enemy == null) return;

        enemy.ApplySlow(slowRate, slowDuration, slowEffectPrefab);

        StartCoroutine(ApplySlowDamageAndReset(enemy.gameObject, damagePerSec, slowDuration));
    }

    private IEnumerator ApplySlowDamageAndReset(GameObject target, float damagePerSecond, float duration)
    {
        yield return StartCoroutine(SlowDamageCoroutine(target, damagePerSecond, duration));
        isFiring = false;
    }
    
    private IEnumerator SlowDamageCoroutine(GameObject target, float damagePerSecond, float duration)
    {
        int tickCount = Mathf.FloorToInt(duration); // 1초 단위로 나누기

        for (int i = 0; i < tickCount; i++)
        {
            yield return new WaitForSeconds(1f); // ✅ 1초마다 데미지 전달

            if (target == null) yield break;

            CombatEvent combatEvent = new CombatEvent
            {
                Sender = this.gameObject,
                Receiver = target,
                Damage = damagePerSecond,
                HitPosition = target.transform.position,
                Collider = target.GetComponent<Collider>()
            };

            CombatSystem.Instance.AddCombatEvent(combatEvent);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}