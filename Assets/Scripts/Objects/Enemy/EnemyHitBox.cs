using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    // 이것도 굳이 스크립트 하나 더 쓸 필요가 있었을까?, BuildingPoint처럼 충돌처리라는 기능 하나만 가진다
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DamageableUnit")) return;
        enemyAttack.OnHit(other);
    }
}