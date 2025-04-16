using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float damage;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;
        
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