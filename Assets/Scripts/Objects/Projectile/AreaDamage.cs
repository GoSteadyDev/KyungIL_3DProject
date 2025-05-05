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
            // main 모듈 가져오기, Unity의 ParticleSystem은 여러 "모듈(Module)"로 구성돼
            // main은 그 중 Main Module을 의미하는데, 파티클의 기본 설정(지속 시간, 시작 속도, 루프 여부 등)을 제어
            ParticleSystem.MainModule mainModule = ps.main;
            // 이펙트의 전체 재생 길이(초) 조회
            float duration = mainModule.duration;
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
}