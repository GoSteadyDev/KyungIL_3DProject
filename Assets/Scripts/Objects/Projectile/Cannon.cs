using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject areaDamagePrefab;
    
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
        
        float distance = dirXZ.magnitude;
        float dynamicArcHeight = Mathf.Clamp(distance * 0.25f, 3f, 12f); // 비율 조정

        float boostedY = dir.y + dynamicArcHeight;

        float Vy = (boostedY + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        // 수평 속도 계산
        Vector3 Vxz = dirXZ / timeToTarget;

        // 최종 velocity
        Vector3 velocity = Vxz + Vector3.up * Vy;
        rb.velocity = velocity;
        
        // ✅ 발사체가 이동 방향을 바라보게 설정
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(90, 0, 0);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Instantiate(areaDamagePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}