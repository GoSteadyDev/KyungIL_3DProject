using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaDamage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damage = 3f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float areaRange = 3f;
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private LayerMask enemyLayer;
    // => 인스펙터에서 안쓰는 애, 굳이 필요 없을 것 같다
    
    private void Start()
    {
        particleEffect = GetComponentInChildren<ParticleSystem>();
        
        // shapeModule은 구조체라서 따로 변수로 받아서 수정해야 함!
        var shape = particleEffect.shape;
        shape.radius = areaRange;
        
        enemyLayer = LayerMask.GetMask("Enemy");
        
        Collider[] hits = Physics.OverlapSphere(transform.position, areaRange, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                CombatSystem.Instance.AddCombatEvent(new CombatEvent
                {
                    Sender = this.gameObject,
                    Receiver = hit.gameObject,
                    Damage = damage,
                    HitPosition = transform.position,
                    Collider = hit
                });
            }
        }

        Destroy(gameObject, duration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaRange);
    }
}
