using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Lazer : MonoBehaviour
{
    private Transform firePoint;
    private Transform target;
    private float damage;
    private float damageInterval;
    private Vector3 boxHalfExtents;
    private Coroutine tickRoutine;
    private LayerMask enemyLayerMask;

    public void Initialize(Transform firePoint, Transform target, float damage, float interval, Vector3 beamBoxHalfExtents)
    {
        this.firePoint        = firePoint;
        this.target           = target;
        this.damage           = damage;
        this.damageInterval   = interval;
        this.boxHalfExtents   = beamBoxHalfExtents;
        // (필요하면) 적 레이어도 전달받도록 시그니처 확장

        var ps = GetComponent<ParticleSystem>();
        ps.Play();

        tickRoutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= damageInterval)
            {
                timer = 0f;
                // OverlapBox로 타겟 주변 박스 범위 내 적 찾아 데미지
                var center = (firePoint.position + target.position) * 0.5f;
                var dir    = (target.position - firePoint.position).normalized;
                var rot    = Quaternion.LookRotation(dir);

                Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, rot, enemyLayerMask);
                
                foreach (var c in hits)
                {
                    CombatSystem.Instance.AddCombatEvent(new CombatEvent
                    {
                        Sender      = gameObject,
                        Receiver    = c.gameObject,
                        Damage      = damage,
                        HitPosition = c.ClosestPoint(firePoint.position),
                        Collider    = c
                    });
                }
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (tickRoutine != null) StopCoroutine(tickRoutine);
    }
}


// private IEnumerator DamageRoutine()
// {
//     float tempTime = 0;
//         
//     while (tempTime < damageInterval)
//     {
//         tempTime += Time.deltaTime;
//             
//         Vector3 center = firePoint.position + transform.forward * (boxSize.z / 2f);
//         Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, transform.rotation, LayerMask.GetMask("Enemy"));
//
//         foreach (var hit in hits)
//         {
//             if (hit.TryGetComponent(out EnemyController enemy))
//             {
//                 CombatSystem.Instance.AddCombatEvent(new CombatEvent
//                 {
//                     Sender = this.gameObject,
//                     Receiver = enemy.gameObject,
//                     Damage = damagePerSecond,
//                     HitPosition = enemy.transform.position,
//                     Collider = hit
//                 });
//             }
//         }
//         yield return new WaitForSeconds(damageInterval);
//     }
// }