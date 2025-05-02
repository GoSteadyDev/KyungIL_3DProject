using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CannonTower : BaseTower
{
    [Header("Fire Points")]
    [SerializeField] private Transform[] firePoints;  // 발사구 위치 배열
    [Header("Enemy Mask")]
    [SerializeField] private LayerMask enemyLayerMask;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void RefreshStats() { /* 캐논은 SO 데이터만 사용하므로 빈 구현 */ }

    protected override Transform FindTarget()
    {
        // 최소 사거리 내 가장 가까운 적 반환
        Collider[] hits = Physics.OverlapSphere(transform.position, data.attackRange, enemyLayerMask);
        if (hits.Length == 0) return null;

        Transform nearest = hits[0].transform;
        float minDist = (nearest.position - transform.position).sqrMagnitude;
        foreach (var c in hits)
        {
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < minDist)
            {
                minDist = d;
                nearest = c.transform;
            }
        }
        return nearest;
    }

    protected override void Attack(Transform target)
    {
        if (data.attackData.burstCount > 1)
            StartCoroutine(FireSequentialBurst(target));
        else
            FireFromPoint(firePoints[0], target);
    }

    private IEnumerator FireSequentialBurst(Transform target)
    {
        animator.SetTrigger("IsAttack");
        
        if (target == null) yield break;
    
        // burstCount 만큼 순차 발사
        for (int i = 0; i < data.attackData.burstCount; i++)
        {
            // 포구 인덱스 순환 (포구 3개면 0→1→2→0→…)
            Transform fp = firePoints[i % firePoints.Length];
            FireFromPoint(fp, target);
            yield return new WaitForSeconds(data.attackData.burstInterval);
        }
    }

    private void FireFromPoint(Transform fp, Transform target)
    {
        if (target == null) return;

        // 예측 위치 계산 (이전 로직 그대로)
        Vector3 enemyPos   = target.position;
        Vector3 shooterPos = fp.position;
        var controller     = target.GetComponent<EnemyController>();
        Vector3 enemyVel   = controller != null ? controller.GetVelocity() : Vector3.zero;
        float speed        = data.attackData.projectileSpeed;
        float flightTime   = Vector3.Distance(enemyPos, shooterPos) / speed;
        float factor       = data.attackData.overshootFactor;
        Vector3 aimPoint   = enemyPos + enemyVel * (flightTime * factor);

        // Instantiate 후 Initialize
        var go = Instantiate(
            data.attackData.projectilePrefab,
            shooterPos,
            Quaternion.identity
        );
        var cannon = go.GetComponent<Cannon>();
        cannon.Initialize(
            shooterPos,
            aimPoint,
            speed,
            flightTime,
            data.damage,
            data.attackData.areaEffectPrefab,
            data.attackData.areaRadius,
            enemyLayerMask
        );
    }
}
