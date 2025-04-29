using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Lazer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damagePerSec;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private Vector3 boxSize = new Vector3(4f, 4f, 80f); 
    [SerializeField] private ParticleSystem lazerEffect;
    
    private Transform target;
    private Transform firePoint;
    private Coroutine tickRoutine;
    
    private void OnDisable()
    {
        if (tickRoutine != null)
        {
            StopCoroutine(tickRoutine);
            tickRoutine = null; // 다음에 Initialize에서 다시 실행 가능
        }

        if (lazerEffect.isPlaying)
            lazerEffect.Stop();
    }
    
    private void Update()
    {
        if (target == null) return;

        // 회전은 레이저 이펙트가 직접 수행
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir + Vector3.forward * 2f);
    }

    public void Initialize(Transform firePoint, Transform targetTransform, float dmg)
    {
        this.firePoint = firePoint;
        target = targetTransform;
        
        damagePerSec = dmg;

        if (!lazerEffect.isPlaying)
            lazerEffect.Play();
        
        if (tickRoutine == null)
            tickRoutine = StartCoroutine(DamageRoutine());
    }
    
    private IEnumerator DamageRoutine()
    {
        float tempTime = 0;
        
        while (tempTime < damageInterval)
        {
            tempTime += Time.deltaTime;
            
            Vector3 center = firePoint.position + transform.forward * (boxSize.z / 2f);
            Collider[] hits = Physics.OverlapBox(center, boxSize / 2f, transform.rotation, LayerMask.GetMask("Enemy"));

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out EnemyController enemy))
                {
                    CombatSystem.Instance.AddCombatEvent(new CombatEvent
                    {
                        Sender = this.gameObject,
                        Receiver = enemy.gameObject,
                        Damage = damagePerSec,
                        HitPosition = enemy.transform.position,
                        Collider = hit
                    });
                }
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + transform.forward * (boxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}