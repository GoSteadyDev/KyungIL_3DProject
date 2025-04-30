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
        // 타워 데이터의 burstCount로 싱글샷/멀티샷 구분
        if (data.attackData.burstCount > 1)
            StartCoroutine(FireBurst(target));
        else
            FireVolley(target);
    }

    private IEnumerator FireBurst(Transform target)
    {
        for (int i = 0; i < data.attackData.burstCount; i++)
        {
            FireVolley(target);
            yield return new WaitForSeconds(data.attackData.burstInterval);
        }
    }

    private void FireVolley(Transform target)
    {
        if (target == null) return;

        // 적의 예측 위치 계산
        Vector3 enemyPos = target.position;
        Vector3 shooterPos = firePoints.Length > 0 ? firePoints[0].position : transform.position;
        var controller = target.GetComponent<EnemyController>();
        Vector3 enemyVel = controller != null ? controller.GetVelocity() : Vector3.zero;
        float speed = data.attackData.projectileSpeed;
        float flightTime = Vector3.Distance(enemyPos, shooterPos) / speed;
        float factor = data.attackData.overshootFactor;
        Vector3 aimPoint = enemyPos + enemyVel * (flightTime * factor);

        // 모든 발사구에서 동시에 발사
        foreach (var fp in firePoints)
        {
            var go = Instantiate(data.attackData.projectilePrefab, fp.position, Quaternion.LookRotation((aimPoint - fp.position).normalized));
            var cannon = go.GetComponent<Cannon>();
            cannon.Initialize(data.attackData, data.damage, aimPoint, enemyLayerMask);
        }
    }
}
