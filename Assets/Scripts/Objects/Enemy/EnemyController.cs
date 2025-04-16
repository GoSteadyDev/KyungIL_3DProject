using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    private Transform castleTarget;
    private Transform[] wayPoints;
    private int wayPointIndex = 0;

    [SerializeField] private float moveSpeed = 25.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float turnDistance = 0.1f;

    private Animator animator;
    public NavMeshAgent navMeshAgent;
    private bool isChasingCastle = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
    }

    void Update()
    {
        if (isChasingCastle)
        {
            MoveToCastle();
        }
        else
        {
            MoveToWaypoints();
        }
    }

    private void MoveToCastle()
    {
        if (!navMeshAgent.enabled) navMeshAgent.enabled = true;
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.SetDestination(castleTarget.position);
    }

    private void MoveToWaypoints()
    {
        if (wayPoints.Length == 0 || wayPointIndex >= wayPoints.Length)
        {
            isChasingCastle = true;
            return;
        }

        Vector3 direction = (wayPoints[wayPointIndex].position - transform.position).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);

        RotateTowards(direction);

        float distance = Vector3.Distance(transform.position, wayPoints[wayPointIndex].position);
        if (distance < turnDistance)
        {
            wayPointIndex++;
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
        moveSpeed = 0;
        // 애니메이션 이벤트 or 코루틴으로 Destroy 처리
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("IsAttack");
    }

    public void SetWayPoints(Transform[] points)
    {
        wayPoints = points;
    }

    public void SetCastleTarget(Transform target)
    {
        castleTarget = target;
    }
}