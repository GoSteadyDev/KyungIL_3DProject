using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform firePoint;
    public Transform FirePoint => firePoint;
    
    private float statDamage, statAttackSpeed, statAttackRange;
    private Transform targetTransform;
    
    private AttackData attackData;
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAttackData(AttackData data)
    {
        attackData = data;
    }

    public void SetStats(float dmg, float speed, float range)
    {
        statDamage      = dmg;
        statAttackSpeed = speed;
        statAttackRange = range;
    }

    public Transform FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position,statAttackRange,LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;

        if (targetTransform != null)
        {
            // 수평 회전만 할 거면 Y 축 고정
            Vector3 dir = targetTransform.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        return targetTransform;
    }

    // 애니메이션 이벤트에서 호출
    public void Attack(Transform target)
    {
        if (target == null) return;

        GameObject arrowObj = Instantiate(attackData.projectilePrefab, firePoint.position, 
            Quaternion.LookRotation((target.position - firePoint.position).normalized));
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Initialize(target, statDamage, attackData.projectileSpeed);
        
        StartAttackAnim();
    }
        
    public void StartAttackAnim()
    {
        animator.SetTrigger("IsAttack");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, statAttackRange);
    }
}

