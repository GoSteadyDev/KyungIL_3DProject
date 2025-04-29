using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private int enemyGold;
    [SerializeField] private float deathTime = 1f;
    
    private float currentHP;
    private bool isDead = false;
    
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        // 골드 지급 등 내부 처리
        ResourceManager.Instance.AddGold(enemyGold);
        MinimapBlipManager.Instance.UnregisterTarget(transform);

        // 💥 KillEvent 발생
        KillEventSystem.Instance.Broadcast(new KillEvent
        {
            Victim = gameObject,
            Position = transform.position,
            GoldReward = enemyGold
        });

        StartCoroutine(DeathTerm());
    }
    
    private IEnumerator DeathTerm()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}

