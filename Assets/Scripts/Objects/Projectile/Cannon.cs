using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
     private float damage;
     private float speed;
     private float areaRadius;
     
     private GameObject areaEffectPrefab;
     private LayerMask enemyLayerMask;
     private Vector3 targetPosition;
     private Rigidbody rb;

     /// <summary>
     /// origin: 발사 위치
     /// aimPoint: 예측 목표 위치
     /// projectileSpeed: 투사체 속도
     /// timeToTarget: 예측 비행 시간
     /// </summary>
     public void Initialize(
         Vector3 origin,
         Vector3 aimPoint,
         float projectileSpeed,
         float timeToTarget,
         float towerDamage,
         GameObject areaEffect,
         float areaRadius,
         LayerMask enemyMask)
     {
         damage           = towerDamage;
         areaEffectPrefab = areaEffect;
         this.areaRadius  = areaRadius;
         enemyLayerMask   = enemyMask;

         rb = GetComponent<Rigidbody>();

         // === 포물선 궤적 계산 ===
         Vector3 dir     = aimPoint - origin;
         Vector3 dirXZ   = new Vector3(dir.x, 0, dir.z);
         float gravity   = Mathf.Abs(Physics.gravity.y);
         float distance  = dirXZ.magnitude;

         // 높이 보정: 거리에 비례한 Arc 높이 (clamp)
         float dynamicArc = Mathf.Clamp(distance * 0.25f, 3f, 12f);

         // 목표 높이 + arc 높이
         float boostedY = dir.y + dynamicArc;

         // 초기 상승 속도 Vy
         float Vy = (boostedY + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

         // 수평 속도 Vxz
         Vector3 Vxz = dirXZ / timeToTarget;

         // 최종 속도 벡터
         Vector3 velocity = Vxz + Vector3.up * Vy;
         rb.velocity = velocity;

         // === 모델 회전 ===
         // 모델 로컬 +Y 축이 전방(forward)이므로,
         // up 벡터 → velocity 방향으로 매핑
         transform.rotation = Quaternion.FromToRotation(Vector3.up, velocity.normalized);
     }

     private void OnCollisionEnter(Collision col)
     {
         // 충돌 시 폭발 생성
         var expGo = Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
         var area = expGo.GetComponent<AreaDamage>();
         area.Initialize(damage, areaRadius, enemyLayerMask);

         Destroy(gameObject);
     }
}

// public Rigidbody rb;
// private Transform target;
// private Vector3 velocity;


// public void SetTarget(Vector3 target, float timeToTarget)
// {
//     rb = GetComponent<Rigidbody>();
//
//     Vector3 dir = target - transform.position;
//     Vector3 dirXZ = new Vector3(dir.x, 0, dir.z);
//
//     float gravity = Mathf.Abs(Physics.gravity.y);
//
//     // 수직 속도 계산 (y 방향으로 위로 올라갔다가 떨어지도록)
//     // 등가속도 운동 공식 활용, 초기 수직 속도 계산
//     
//     float distance = dirXZ.magnitude;
//     float dynamicArcHeight = Mathf.Clamp(distance * 0.25f, 3f, 12f); // 비율 조정
//
//     float boostedY = dir.y + dynamicArcHeight;  // 최고 높이
//
//     float Vy = (boostedY + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;
//
//     // 수평 속도 계산
//     Vector3 Vxz = dirXZ / timeToTarget;
//
//     // 최종 velocity
//     Vector3 velocity = Vxz + Vector3.up * Vy;
//     rb.velocity = velocity;
//     
//     // ✅ 발사체가 이동 방향을 바라보게 설정
//     if (velocity != Vector3.zero)
//     {
//         transform.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(90, 0, 0);
//     }
// }
//
// private void OnCollisionEnter(Collision other)
// {
//     Instantiate(areaDamagePrefab, transform.position, Quaternion.identity);
//     Destroy(gameObject);
// }

// private Vector3 CalculateBallisticVelocity(Vector3 origin, Vector3 target, float launchSpeed)
// {
//     Vector3 toTarget = target - origin;
//     float g = Physics.gravity.y;
//     float y = toTarget.y;
//     toTarget.y = 0;
//     float x = toTarget.magnitude;
//     float speed2 = launchSpeed * launchSpeed;
//     float underRoot = speed2 * speed2 - g * (g * x * x + 2 * y * speed2);
//     
//     if (underRoot < 0)
//         return toTarget.normalized * launchSpeed; // 직선 발사
//
//     float root = Mathf.Sqrt(underRoot);
//     float highAngle = (speed2 + root) / (-g * x);
//     float angle = Mathf.Atan(highAngle);
//     
//     Vector3 dir = toTarget.normalized;
//     
//     return new Vector3(
//         dir.x * launchSpeed * Mathf.Cos(angle),
//         launchSpeed * Mathf.Sin(angle),
//         dir.z * launchSpeed * Mathf.Cos(angle)
//     );
// }