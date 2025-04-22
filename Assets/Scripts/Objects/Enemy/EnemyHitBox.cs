using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DamageableUnit")) return;
        enemyAttack.OnHit(other);
    }
}