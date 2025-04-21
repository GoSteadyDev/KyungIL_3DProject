using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private int enemyGold;
    
    
    public float MaxHP => maxHP;
    
    private float currentHP;
    public float CurrentHP => currentHP;

    private EnemyController enemyController;

    [SerializeField] private float deathTime = 1f;
    private bool isDead = false;
    
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        currentHP = Mathf.Clamp( currentHP, 0, maxHP);
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

