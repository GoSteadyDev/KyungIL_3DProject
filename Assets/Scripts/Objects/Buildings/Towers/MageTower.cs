using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MageTower : BaseTower
{
    [Header("Effect Origin")]
    [SerializeField] private Transform effectPoint;     // (레이저만) 쏘는 위치

    [Header("Child Effects")]
    [SerializeField] private Lazer lazerChild;           // 자식 레이저 이펙트
    [SerializeField] private ParticleSystem attackEffect; // AoE 슬로우 Lv3A용 이펙트

    [Header("Enemy Mask")]
    [SerializeField] private LayerMask enemyLayerMask;

    private Animator animator;
    private bool isFiring;
    
    public override string GetDescription()
    {
        var attackData = data.attackData;

        // Beam (LazerTower, Lv3B), F1, F2는 숫자 서식 지정자로 소수점 자리 표현
        if (attackData.attackType == AttackType.Beam)
        {
            float dps = data.damage;
            float tick = attackData.beamInterval;
            return $"\n- DPS: {dps:F1}\n" +
                   $"- Tick Interval: {tick:F2}s\n" +
                   $"- Range: {data.attackRange}";
        }

        // AoE Slow (Lv3A)
        if (attackData.areaRadius > 0f)
        {
            return $"- Slow Rate: {attackData.slowRate * 100f:F0}%\n" +
                   $"- Slow Duration: {attackData.slowDuration:F1}s\n" +
                   $"- Area Radius: {attackData.areaRadius:F1}\n" +
                   $"- Damage: {data.damage}\n" +
                   $"- Range: {data.attackRange}\n" +
                   $"- Attack Speed: {data.attackSpeed:F2}";
        }

        // 기본 단일 슬로우
        return $"\n" +
               $"- Slow Rate: {attackData.slowRate * 100f:F0}%\n" +
               $"- Slow Duration: {attackData.slowDuration:F1}s\n" +
               $"- Damage: {data.damage}\n" +
               $"- Range: {data.attackRange}\n" +
               $"- Attack Speed: {data.attackSpeed:F2}";
    }
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        // 시작 시 레이저 비활성화
        if (lazerChild != null)
        {
            lazerChild.Stop();
            lazerChild.gameObject.SetActive(false);
        }
        // Lv3A의 이펙트도 비활성화
        if (attackEffect != null) attackEffect.Stop();
    }

    protected override void Update()
    {
        // 1) Beam 모드 처리
        if (data.attackData.attackType == AttackType.Beam)
        {
            HandleBeamMode();
            return;
        }

        // 2) Direct 모드(슬로우)일 때만 BaseTower.Update() 실행
        base.Update();
    }

    protected override Transform FindTarget()
    {
        // 살아있는 적만 골라서 가장 가까운 놈 반환
        Collider[] hits = Physics.OverlapSphere(transform.position, data.attackRange, enemyLayerMask);
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var col in hits)
        {
            var hp = col.GetComponent<EnemyHP>();
            
            if (hp != null && hp.IsDead) continue;

            float distance = (col.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    // Direct 모드만 처리, Lazer는 오버라이드 된 Update에서
    protected override void Attack(Transform target)
    {
        if (data.attackData.attackType != AttackType.Direct || target == null)
            return;

        animator.SetTrigger("IsAttack");

        if (data.attackData.areaRadius > 0f)
        {
            // Lv3A 범위 슬로우
            if (isFiring == false) StartCoroutine(FireSlowAreaRoutine());
        }
        else
        {
            // Lv1~2 단일 슬로우
            if (isFiring == false) StartCoroutine(FireSlowSingleRoutine(target));
        }
    }

    private IEnumerator FireSlowSingleRoutine(Transform target)
    {
        isFiring = true;
        // 중복 코루틴 실행 방지. 없으면 1초 간격으로 데미지를 주는 여러 코루틴이 동시에 실행되며, 1초에 여러 번씩 데미지가 중첩 적용

        var enemyController = target.GetComponent<EnemyController>();
        enemyController?.ApplySlow(data.attackData.slowRate, data.attackData.slowDuration, data.attackData.slowEffectPrefab);
        // ? -> null 조건 연산자, enemyController가 null이 아니면 메서드를 실행하고, null이면 아무 것도 하지 않음

        float total = data.attackData.slowDuration;
        int ticks = Mathf.FloorToInt(total);
        float tickDmg = data.damage / ticks;

        for (int i = 0; i < ticks; i++)
        {
            yield return new WaitForSeconds(1f);
            if (target == null) break;

            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender      = this.gameObject,
                Receiver    = target.gameObject,
                Damage      = tickDmg,
                HitPosition = target.position,
                Collider    = target.GetComponent<Collider>()
            });
        }

        isFiring = false;
    }

    private IEnumerator FireSlowAreaRoutine()
    {
        isFiring = true;

        attackEffect?.Play();
        yield return new WaitForSeconds(0.1f);

        Collider[] hits = Physics.OverlapSphere(transform.position, data.attackData.areaRadius, enemyLayerMask);
        
        foreach (var col in hits)
        {
            var enemyController = col.GetComponent<EnemyController>();
            enemyController?.ApplySlow(
                data.attackData.slowRate,
                data.attackData.slowDuration,
                data.attackData.slowEffectPrefab
            );
            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender      = this.gameObject,
                Receiver    = enemyController.gameObject,
                Damage      = data.damage,
                HitPosition = enemyController.transform.position,
                Collider    = col
            });
        }

        yield return new WaitForSeconds(1f / data.attackSpeed);
        isFiring = false;
    }
    
    private void HandleBeamMode()
    {
        Transform target = FindTarget();
        
        if (target != null)
        {
            animator.SetTrigger("IsAttack");
            
            if (lazerChild.gameObject.activeSelf == false)
            {
                // 위치·회전 갱신 후 초기화
                lazerChild.transform.position = effectPoint.position;
                lazerChild.transform.rotation = Quaternion.LookRotation((target.position - effectPoint.position).normalized);
                lazerChild.gameObject.SetActive(true);
                lazerChild.Initialize(
                    effectPoint,
                    target,
                    data.damage,
                    data.attackData,
                    enemyLayerMask
                );
            }
        }
        else if (lazerChild.gameObject.activeSelf)
        {
            // 타겟 소실 시 즉시 비활성화
            lazerChild.Stop();
        }
    }
}
