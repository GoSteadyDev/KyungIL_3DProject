using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] wayPoints;
    private int wayPointIndex = 0;

    [SerializeField] private float moveSpeed = 25.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float turnDistance = 0.1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (wayPoints.Length == 0 || wayPointIndex >= wayPoints.Length)
            return;

        // 앞으로 이동
        Vector3 direction = (wayPoints[wayPointIndex].position - transform.position).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);

        // 부드러운 회전
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
        
        // 턴 계산하기
        float distance = Vector3.Distance(transform.position, wayPoints[wayPointIndex].position);
        if (distance < turnDistance)
        {
            wayPointIndex++;
        }
    }
    
    public void PlayDeath()
    {
        animator.SetTrigger("IsDead");
        moveSpeed = 0;
        // 애니메이션 이벤트 or 코루틴으로 Destroy 처리
    }
    
    public void SetWayPoints(Transform[] points)
    {
        wayPoints = points;
    }
}