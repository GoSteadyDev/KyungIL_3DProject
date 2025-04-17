using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private Collider fireRange;
    [SerializeField] private ParticleSystem fireParticle;
    
    private Rigidbody rigidbody;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        
        CombatEvent combatEvent = new CombatEvent
        {
            Sender = this.gameObject,
            Receiver = other.gameObject,
            Damage = damage,
            HitPosition = transform.position,
            Collider = other
        };

        CombatSystem.Instance.AddCombatEvent(combatEvent);
    }
}
