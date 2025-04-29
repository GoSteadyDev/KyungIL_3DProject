using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float turnDistance = 0.5f;
    
    private Animator animator;
    public NavMeshAgent navMeshAgent;
    
    private int wayPointIndex = 0;
    private bool isChasingCastle = false;
    private bool isSlowed = false;
    
    private Coroutine slowCoroutine;
    private GameObject activeSlowEffect;
    
    private Transform[] wayPoints;
    private Transform castleTarget;
    
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
        if (castleTarget == null || GameManager.Instance.IsGameOver) return;

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
    
    public Vector3 GetVelocity()
    {
        if (navMeshAgent.enabled)
            return navMeshAgent.velocity;

        Vector3 direction = (wayPoints[wayPointIndex].position - transform.position).normalized;
        return direction * moveSpeed;
    }
    
    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }
    
   public void ApplySlow(float slowRate, float duration, GameObject effectPrefab)
   {
       if (isSlowed) return;

       slowCoroutine = StartCoroutine(SlowRoutine(slowRate, duration, effectPrefab));
   }

   private IEnumerator SlowRoutine(float rate, float duration, GameObject effectPrefab)
   {
       isSlowed = true;
       float originalSpeed = moveSpeed;

       moveSpeed *= (1f - rate); // 슬로우 적용

       // 이펙트 생성 후 적에게 붙이기
       if (effectPrefab != null)
       {
           activeSlowEffect = Instantiate(effectPrefab, transform.position + Vector3.up * 7.5f, Quaternion.Euler(-90f, 0f, 0f));
           activeSlowEffect.transform.SetParent(transform); // 따라다니게
       }

       yield return new WaitForSeconds(duration);

       moveSpeed = originalSpeed; // 속도 복구
       isSlowed = false;

       if (activeSlowEffect != null)
           Destroy(activeSlowEffect); // 이펙트 제거
   }
}