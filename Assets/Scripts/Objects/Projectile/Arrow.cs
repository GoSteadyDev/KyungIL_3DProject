using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private LayerMask enemyLayerMask;
    
    private float damage;
    private Transform target;
    private bool hasHit = false;

    public float Damage => damage;

     public void Initialize(Transform newTarget, float newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed  = newSpeed;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position);

        if (direction != Vector3.zero)
        {
            direction.Normalize();
            transform.position += direction * (speed * Time.deltaTime);
            transform.forward = direction;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // ✅ 이미 처리했다면 무시

        // 레이어 마스크 검사
        if ((enemyLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            hasHit = true; // ✅ 이후엔 무시되도록 플래그 설정

            // 데미지 이벤트 실행
            CombatSystem.Instance.AddCombatEvent(new CombatEvent
            {
                Sender = this.gameObject,
                Receiver = other.gameObject,
                Damage = damage,
                HitPosition = transform.position,
                Collider = other
            });

            Destroy(gameObject); // 마지막에 제거
        }
    }
}