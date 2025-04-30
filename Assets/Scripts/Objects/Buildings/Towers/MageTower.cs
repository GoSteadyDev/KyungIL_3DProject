using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTower : BaseTower
{
    [Header("Effect Origin")]
    [SerializeField] private Transform effectPoint;
    [SerializeField] private ParticleSystem attackEffectPrefab;
    
    [Header("Enemy Mask")]
    [SerializeField] private LayerMask enemyLayerMask;

    protected override void RefreshStats()
    {
        // (필요 시) effectPoint 위치·머티리얼 조절
    }

    protected override Transform FindTarget()
    {
        // BaseTower에는 구현이 없으니, 사거리 내 가장 가까운 적 직접 찾기
        Collider[] hits = Physics.OverlapSphere(transform.position, data.attackRange, enemyLayerMask);
        
        if (hits.Length == 0) return null;
        
        Transform nearest = hits[0].transform;
        float minD = (nearest.position - transform.position).sqrMagnitude;
        
        foreach (var c in hits)
        {
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < minD) { d = minD; nearest = c.transform; }
        }
        
        return nearest;
    }
    
    protected override void Attack(Transform target)
    {
        switch (data.attackData.attackType)
        {
            case AttackType.Beam:
                FireBeam(target);
                break;
            case AttackType.Direct:
                // areaRadius 가 0 이면 단일 슬로우, 0 초과면 광역 AoE 슬로우
                if (data.attackData.areaRadius > 0f)
                    FireSlowArea();
                else
                    FireSlowSingle(target);
                break;
        }
    }

    /// <summary>Lv1~3A: 단일 슬로우 + 틱 데미지</summary>
    private void FireSlowSingle(Transform target)
    {
        if (target == null) return;
        
        var ec = target.GetComponent<EnemyController>();
        if (ec == null) return;

        // 1) 단일 슬로우 이펙트
        GameObject slowGo = Instantiate(data.attackData.slowEffectPrefab, // 기존 slowEffectPrefab
            target.position, Quaternion.identity);
        // (이펙트 크기/위치는 각 TowerData 에 맞춰 SO 에 설정)

        // 2) ApplySlow
        ec.ApplySlow(data.attackData.slowRate, data.attackData.slowDuration, data.attackData.slowEffectPrefab);

        // 3) 틱 데미지 (TowerData.damage을 초당 1틱씩 나눠 입힘)
        float totalDuration = data.attackData.slowDuration;
        int  tickCount      = Mathf.FloorToInt(totalDuration);      // 예: 3초 → 3틱
        float tickDamage    = data.damage / tickCount;             // 예: damage=3 → 1씩
        StartCoroutine(ApplyTickDamage(target.gameObject, tickDamage, tickCount));
    }

    private IEnumerator ApplyTickDamage(GameObject target, float tickDmg, int ticks)
    {
        for (int i = 0; i < ticks; i++)
        {
            yield return new WaitForSeconds(1f);
            if (target == null) yield break;
            CombatSystem.Instance.AddCombatEvent(new CombatEvent {
                Sender      = this.gameObject,
                Receiver    = target,
                Damage      = tickDmg,
                HitPosition = target.transform.position,
                Collider    = target.GetComponent<Collider>()
            });
        }
    }
    
    private void FireSlowArea()
    {
        if (attackEffectPrefab != null) attackEffectPrefab.Play();
        
        // 1) 이펙트
        GameObject slowGo = Instantiate(data.attackData.areaEffectPrefab, effectPoint.position, Quaternion.identity);
        
        var area = slowGo.GetComponent<AreaDamage>();
        area.Initialize( data.damage, data.attackData.areaRadius, enemyLayerMask);

        // 2) 슬로우 효과(EnemyController에 메서드가 있을 경우)
        Collider[] hits = Physics.OverlapSphere(transform.position, data.attackData.areaRadius, enemyLayerMask);
        foreach (var c in hits)
        {
            var ec = c.GetComponent<EnemyController>();
            ec?.ApplySlow(
                data.attackData.slowRate,
                data.attackData.slowDuration,
                data.attackData.areaEffectPrefab
            );
        }
    }
    
    private void FireBeam(Transform target)
    {
        if (target == null) return;

        // 1) 이펙트 켜기
        GameObject beamGo = Instantiate(data.attackData.beamEffectPrefab,
            effectPoint.position, Quaternion.LookRotation((target.position - effectPoint.position).normalized)
        );
        
        var lazer = beamGo.GetComponent<Lazer>();
        lazer.Initialize(effectPoint, target, data.damage, 1f / data.attackSpeed, data.attackData.beamBoxHalfExtents);
    }
}