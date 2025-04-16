using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SwordMan : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float searchRange = 10f;
    [SerializeField] private Transform warfGate;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 3.5f;
    
    private Collider damgeCollider;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    private float nextAttackTime;
    private Transform target;
    private float targetdistance;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        FindTarget();
        
        if (target == null)
        {
            // 워프게이트로 이동
            MoveTo(warfGate.position);
        }

        if (target != null)
        {
            targetdistance = Vector3.Distance(transform.position, target.position);
            StopMoving();
            
            if (targetdistance > attackRange)
            {
                MoveToTarget();
            }
            else
            {
                AttackToTarget();
            }
        }
    }

    private void FindTarget()
    {
        // 공격 범위 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Enemy")); // 탐색 범위 넉넉하게
        target = hits.Length > 0 ? hits[0].transform : null;
    }
    
    private void MoveTo(Vector3 destination)
    {
        animator.SetTrigger("IsWalk");

        if (navMeshAgent.destination != destination)
            navMeshAgent.SetDestination(destination);

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.isStopped = false;
    }
    
    private void StopMoving()
    {
        if (!navMeshAgent.isStopped)
            navMeshAgent.isStopped = true;
    }
    
    private void MoveToTarget()
    {
        animator.SetTrigger("IsWalk");
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // 수평 이동만

        transform.position += direction * (moveSpeed * Time.deltaTime);
        transform.forward = direction; // 타겟 바라보게
    }

    private void AttackToTarget()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackSpeed;
            StartAttackAnim();
        }
    }

    private void StartAttackAnim()
    {
        animator.SetTrigger("IsAttack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
}

