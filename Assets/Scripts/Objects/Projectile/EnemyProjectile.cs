using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    private Transform target;
    private float damage;

    public void Initialize(Transform targetTransform, float dmg)
    {
        target = targetTransform;
        damage = dmg;
        Destroy(gameObject, 5f); // 너무 오래 남아있지 않게
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DamageableUnit")) return;

        CombatEvent combatEvent = new CombatEvent
        {
            Sender = this.gameObject,
            Receiver = other.gameObject,
            Damage = damage,
            HitPosition = transform.position,
            Collider = other
        };

        CombatSystem.Instance.AddCombatEvent(combatEvent);
        Destroy(gameObject);
    }
}
