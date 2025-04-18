using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private float areaRange = 3f;
    [SerializeField] private GameObject areaDamagePrefab;
    [SerializeField] private float rotateSpeed = 10f;     // 유도 강도 조절
    [SerializeField] private float arcHeight = 4f;       // 고정된 포물선 높이
    
    public Rigidbody rb;
    private Transform target;

    private Vector3 velocity;

    public void SetTarget(Vector3 target, float timeToTarget)
    {
        rb = GetComponent<Rigidbody>();

        Vector3 dir = target - transform.position;
        Vector3 dirXZ = new Vector3(dir.x, 0, dir.z);

        float gravity = Mathf.Abs(Physics.gravity.y);

        // 수직 속도 계산 (y 방향으로 위로 올라갔다가 떨어지도록)
        // 등가속도 운동 공식 활용, 초기 수직 속도 계산
        
        float extraArcHeight = 6f;
        float boostedY = dir.y + extraArcHeight;
        
        float Vy = (boostedY + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        // 수평 속도 계산
        Vector3 Vxz = dirXZ / timeToTarget;

        // 최종 velocity
        Vector3 velocity = Vxz + Vector3.up * Vy;

        rb.velocity = velocity;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Instantiate(areaDamagePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}