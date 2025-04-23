using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour, ISelectable, IHasInfoPanel
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float searchRange = 10f;
    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform startPoint; // waypoint07
    [SerializeField] private Transform endPoint;   // warfGate

    private int patrolIndex = 0;
    private bool isInitialPath = true;
    private Vector3[] patrolPath;

    
    [Header("Attack Settings")]
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 3.5f;
    
    [Header("Info Settings")]
    [SerializeField] private Sprite icon;
    
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public string GetDisplayName() => "SwordMan";
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    
    private Collider col;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    private float nextAttackTime;
    private float targetdistance;
    private Transform target;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // 반복 경로 초기화: waypoint07 <-> warfGate 루프
        patrolPath = new Vector3[]
        {
            startPoint.position,
            endPoint.position
        };

        // 첫 이동: spawnPoint → startPoint
        MoveTo(startPoint.position);
    }
    
    private void Update()
    {
        FindTarget();

        if (target == null)
        {
            PatrolMovement();
        }
        else
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

    private void PatrolMovement()
    {
        if (navMeshAgent.remainingDistance <= 0.2f && !navMeshAgent.pathPending)
        {
            // 최초 경로 처리
            if (isInitialPath)
            {
                if (patrolIndex == 0)
                {
                    patrolIndex++;
                    MoveTo(endPoint.position);
                }
                else
                {
                    isInitialPath = false;
                    patrolIndex = 1;
                    MoveTo(startPoint.position);
                }
            }
            else
            {
                patrolIndex = (patrolIndex + 1) % patrolPath.Length;
                MoveTo(patrolPath[patrolIndex]);
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
        StartAttackAnim();
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

