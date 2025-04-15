using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("Attack Settings")] 
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    private Animator animator;
    private float nextAttackTime;
    private Transform target;
    private float targetdistance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        FindTarget();
        
        if (target != null)
        {
            //transform.LookAt(target.position);
            // 수평만 바라보도록
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            
            if (dir != Vector3.zero)
                transform.forward = dir.normalized;
            
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackSpeed;
                FireArrow();
            }
            targetdistance = Vector3.Distance(transform.position, target.position);
        }
        
        
        if (targetdistance > attackRange)
            target = null;
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        target = hits.Length > 0 ? hits[0].transform : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
    public void StartAttackAnim()
    {
        animator.SetTrigger("IsAttack");
    }

    // 애니메이션 이벤트에서 호출
    public void FireArrow()
    {
        if (target == null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.SetTarget(target);
        
        StartAttackAnim();
    }
}

