using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Lazer : MonoBehaviour
{
    private Transform firePoint;        // 레이저 시작 위치(타워의 effectPoint)
    private Transform target;           // 레이저 목표(적)
    private float damage;               // 초당 데미지
    private float damageInterval;       // 데미지 틱 간격(초), 여기서는 1초
    private Vector3 boxHalfExtents;     // OverlapBox 절반 크기
    private LayerMask enemyLayerMask;   // 적 레이어 마스크
    private Coroutine tickRoutine;      // 데미지 반복 코루틴
    private ParticleSystem ps;          // 파티클 시스템

    // 파티클 지속 시간(Inspector에서 설정한 Duration)을 외부에서 읽기 위함
    public float Duration => ps.main.duration;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        // 처음엔 재생되지 않도록
        ps.Stop();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // 이펙트가 켜져 있고, 타겟이 남아 있으면 매 프레임 위치·회전 업데이트
        if (gameObject.activeSelf && target != null && firePoint != null)
        {
            transform.position = firePoint.position;
            transform.rotation = Quaternion.LookRotation((target.position - firePoint.position).normalized);
        }
    }
    
    /// <summary>
    /// 레이저 발사 초기화
    /// </summary>
    /// <param name="firePoint">레이저 시작 위치 Transform</param>
    /// <param name="target">레이저를 향할 적의 Transform</param>
    /// <param name="damage">초당 데미지</param>
    /// <param name="damageInterval">데미지 틱 간격(초)</param>
    /// <param name="boxHalfExtents">OverlapBox 절반 크기</param>
    /// <param name="enemyLayerMask">적 레이어 마스크</param>
    public void Initialize(Transform firePoint, Transform target, float damage,
        float damageInterval, Vector3 boxHalfExtents, LayerMask enemyLayerMask)
    {
        this.firePoint        = firePoint;
        this.target           = target;
        this.damage           = damage;
        this.damageInterval   = damageInterval;
        this.boxHalfExtents   = boxHalfExtents;
        this.enemyLayerMask   = enemyLayerMask;

        // 즉시 위치·회전 맞추고
        transform.position = firePoint.position;
        transform.rotation = Quaternion.LookRotation((target.position - firePoint.position).normalized);
        
        ps.Play();
        gameObject.SetActive(true);

        // 기존 코루틴이 있으면 중지
        if (tickRoutine != null) StopCoroutine(tickRoutine);
        tickRoutine = StartCoroutine(DamageRoutine());
    }

    /// <summary>
    /// 레이저 중지
    /// </summary>
    public void Stop()
    {
        if (tickRoutine != null)
            StopCoroutine(tickRoutine);
        if (ps != null)    // ps가 할당되었을 때만 호출
            ps.Stop();
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 첫 즉시 데미지 + 틱 데미지 반복
    /// </summary>
    private IEnumerator DamageRoutine()
    {
        // 첫 즉시 데미지
        ApplyDamageTick();

        // target이 살아있는 동안에만 반복
        while (target != null)
        {
            yield return new WaitForSeconds(damageInterval);

            if (target == null) break;
            
            ApplyDamageTick();
        }

        Stop();
    }

    /// <summary>
    /// 한 틱마다 OverlapBox로 데미지 이벤트 실행
    /// </summary>
    private void ApplyDamageTick()
    {
        Vector3 center = (firePoint.position + target.position) * 0.5f;
        Vector3 dir    = (target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, rot, enemyLayerMask);
        foreach (var c in hits)
        {
            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender      = this.gameObject,
                Receiver    = c.gameObject,
                Damage      = damage * damageInterval,    // 초당 데미지 × 간격
                HitPosition = c.ClosestPoint(firePoint.position),
                Collider    = c
            });
        }
    }
    
    /// <summary>
    /// 씬 뷰에서 OverlapBox 범위를 확인할 수 있도록 기즈모 그리기
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (firePoint == null || target == null) return;

        Vector3 center = (firePoint.position + target.position) * 0.5f;
        Quaternion rot = Quaternion.LookRotation((target.position - firePoint.position).normalized);

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(center, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}