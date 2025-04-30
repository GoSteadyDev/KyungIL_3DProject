using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaDamage : MonoBehaviour
{
    private float damage;
    private float radius;
    private LayerMask enemyLayer;

    public void Initialize(float dmg, float areaRange, LayerMask mask)
    {
        damage     = dmg;
        radius     = areaRange;
        enemyLayer = mask;

        // 파티클 모듈 크기 조절
        var particleEffect = GetComponentInChildren<ParticleSystem>();
        if (particleEffect != null)
        {
            var shape = particleEffect.shape;
            shape.radius = radius;
        }

        // 즉시 폭발 처리
        ApplyAreaDamage();
        var ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            // main 모듈 가져오기
            var mainModule = ps.main;
            // 이펙트의 전체 재생 길이(초) 조회
            float duration = mainModule.duration;
            // 또는 파티클 1세트 생명주기
            float lifetime = mainModule.startLifetime.constant;
            // 원하는 값으로 Destroy 시간 결정
            Destroy(gameObject, duration);
        }
    }

    private void ApplyAreaDamage()
    {
        var hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        foreach (var col in hits)
        {
            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender      = gameObject,
                Receiver    = col.gameObject,
                Damage      = damage,
                HitPosition = transform.position,
                Collider    = col
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}