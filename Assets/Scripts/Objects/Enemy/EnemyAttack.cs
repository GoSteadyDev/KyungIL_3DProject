using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    
    [SerializeField] private float damage;
    [SerializeField] private float searchRange = 5f;
    [SerializeField] private float attackInterval = 2.5f;
    [SerializeField]private float attackSpeed;
    
    private EnemyController enemyController;
    private Transform attackTarget;
    
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void Update()
    {
        FindTarget();
        
        if (attackTarget != null)
        {
            RotateTowardsTarget();           
            enemyController.PlayAttackAnimation();
            enemyController.enabled = false;
        }
        else
        {
            enemyController.enabled = true;
        }
    }

    public void OnHit(Collider other)
    {
        CombatEvent combatEvent = new CombatEvent
        {
            Sender = this.gameObject,
            Receiver = other.gameObject,
            HitPosition = transform.position,
            Damage = damage,
            Collider = other
        };

        CombatSystem.Instance.AddCombatEvent(combatEvent);
    }
    
    private void FindTarget()
    {
        // 공격 범위 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("DamageableUnit")); // 탐색 범위 넉넉하게
        attackTarget = hits.Length > 0 ? hits[0].transform : null;
    }
    
    private void RotateTowardsTarget()
    {
        Vector3 dir = attackTarget.position - transform.position;
        dir.y = 0f; // y축 고정 (회전이 평면상에서만 이루어지게)
    
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = targetRotation;
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
}
