using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour, IHasRangeUI, IHasInfoPanel
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float searchRange = 10f;
    
    private Transform startPoint; // waypoint07
    private Transform endPoint;   // warfGate
    private int patrolIndex = 0;
    private Vector3[] patrolPath;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3.5f;
    
    [Header("Info Settings")]
    [SerializeField] private Sprite icon;
    
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public string GetDisplayName() => "SwordMan";
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private float targetdistance;
    private Transform target;
    
    private enum UnitState
    {
        Idle,
        InitialMove,    // spawn → startPoint → endPoint
        Patrol,         // startPoint ↔ endPoint
        Combat
    }
    private UnitState currentState = UnitState.InitialMove;
    
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
        if (target == null)
            FindTarget();

        switch (currentState)
        {
            case UnitState.InitialMove:
            case UnitState.Patrol:
                HandleMovement();
                break;
            case UnitState.Combat:
                StopMoving();
                HandleCombat();
                break;
        }
    }
    
    public void SetWayPath(Transform startpos, Transform endpos)
    {
        startPoint = startpos;
        endPoint = endpos;
    }
    
    private void HandleMovement()
    {
        if (target != null)
        {
            currentState = UnitState.Combat;
            StopMoving();
            return;
        }

        if (navMeshAgent.remainingDistance <= 0.2f && !navMeshAgent.pathPending)
        {
            switch (currentState)
            {
                case UnitState.InitialMove:
                    if (patrolIndex == 0)
                    {
                        patrolIndex++;
                        MoveTo(endPoint.position);
                    }
                    else
                    {
                        currentState = UnitState.Patrol;
                        patrolIndex = 1;
                        MoveTo(startPoint.position);
                    }
                    break;

                case UnitState.Patrol:
                    patrolIndex = (patrolIndex + 1) % patrolPath.Length;
                    MoveTo(patrolPath[patrolIndex]);
                    break;
            }
        }
    }

    private void HandleCombat()
    {
        if (target == null)
        {
            currentState = UnitState.Patrol;
            MoveTo(patrolPath[patrolIndex]);
            return;
        }

        float targetDistance = Vector3.Distance(transform.position, target.position);

        if (targetDistance > attackRange)
        {
            MoveToTarget(); // 네비게이션 방식으로 바꾸면 여기 수정
        }
        else
        {
            StartAttackAnim();
        }
    }
    
    private void MoveToTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 stopPosition = target.position - direction * (attackRange - 0.1f); // 조금 덜 가까이 감

        MoveTo(stopPosition);
    }

    private void FindTarget()
    {
        // 공격 범위 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Enemy")); // 탐색 범위 넉넉하게
        target = hits.Length > 0 ? hits[0].transform : null;
    }
    
    private void MoveTo(Vector3 destination)
    {
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