using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyHitBox처럼 무기 HitBox에 붙여둔 컴포넌트
public class UnitAttack : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") == false) return;
        
        CombatEvent combatEvent = new CombatEvent
        {
            Sender = this.gameObject,
            Receiver = other.gameObject,
            HitPosition = transform.position,
            Damage = damage,
            Collider = other
        };

        CombatSystem.Instance.AddCombatEvent(combatEvent);
    }
}