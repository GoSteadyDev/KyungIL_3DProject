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
     private Rigidbody rb;

     private void Awake()
     {
         rb = GetComponent<Rigidbody>();
     }

     /// <summary>
     /// origin: 발사 위치
     /// aimPoint: 예측 목표 위치
     /// projectileSpeed: 투사체 속도
     /// timeToTarget: 예측 비행 시간
     /// </summary>
     public void Initialize(Vector3 origin, Vector3 aimPoint, float timeToTarget,
         float towerDamage, AttackData attackData, LayerMask mask)
     {
         this.damage           = towerDamage;
         this.areaEffectPrefab = attackData.areaEffectPrefab;
         this.areaRadius       = attackData.areaRadius;
         this.enemyLayerMask   = mask;

         this.speed = attackData.projectileSpeed;

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