using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MageTower : BaseTower
{
    [Header("Effect Origin")]
    [SerializeField] private Transform effectPoint;

    [Header("Child Effects")]
    [SerializeField] private Lazer lazerChild;           // 자식 레이저 이펙트
    [SerializeField] private ParticleSystem attackEffect; // AoE 슬로우 Lv3A용 이펙트

    [Header("Enemy Mask")]
    [SerializeField] private LayerMask enemyLayerMask;

    private Animator animator;
    private bool isFiring;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // 시작 시 비활성화
        // 초기 이펙트 비활성화
        if (lazerChild != null)
        {
            lazerChild.Stop();
            lazerChild.gameObject.SetActive(false);
        }
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
        float minD = float.MaxValue;

        foreach (var c in hits)
        {
            var hp = c.GetComponent<EnemyHP>();
            if (hp != null && hp.IsDead) continue;

            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < minD)
            {
                minD = d;
                nearest = c.transform;
            }
        }

        return nearest;
    }

    // Direct 모드만 처리
    protected override void Attack(Transform target)
    {
        if (data.attackData.attackType != AttackType.Direct || target == null)
            return;

        animator.SetTrigger("IsAttack");

        if (data.attackData.areaRadius > 0f)
        {
            // Lv3A AoE 슬로우
            if (!isFiring) StartCoroutine(FireSlowAreaRoutine());
        }
        else
        {
            // Lv1~2 단일 슬로우
            if (!isFiring) StartCoroutine(FireSlowSingleRoutine(target));
        }
    }

    private IEnumerator FireSlowSingleRoutine(Transform target)
    {
        isFiring = true;

        var ec = target.GetComponent<EnemyController>();
        ec?.ApplySlow(
            data.attackData.slowRate,
            data.attackData.slowDuration,
            data.attackData.slowEffectPrefab
        );

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

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            data.attackData.areaRadius,
            enemyLayerMask
        );
        foreach (var c in hits)
        {
            var ec = c.GetComponent<EnemyController>();
            ec?.ApplySlow(
                data.attackData.slowRate,
                data.attackData.slowDuration,
                data.attackData.slowEffectPrefab
            );
            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender      = this.gameObject,
                Receiver    = ec.gameObject,
                Damage      = data.damage,
                HitPosition = ec.transform.position,
                Collider    = c
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
            if (!lazerChild.gameObject.activeSelf)
            {
                // 위치·회전 갱신 후 초기화
                lazerChild.transform.position = effectPoint.position;
                lazerChild.transform.rotation = Quaternion.LookRotation((target.position - effectPoint.position).normalized);
                lazerChild.gameObject.SetActive(true);
                lazerChild.Initialize(
                    effectPoint,
                    target,
                    data.damage,
                    data.attackData.beamInterval,
                    data.attackData.beamBoxHalfExtents,
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
