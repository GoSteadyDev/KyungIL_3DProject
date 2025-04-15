using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float damage;
    [SerializeField] private float speed;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target == null)
        {
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


