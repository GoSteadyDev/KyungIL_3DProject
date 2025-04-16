using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider hpViewerPrefab;
    [SerializeField] private float maxHP = 10f;
    public float MaxHP => maxHP;
    
    private float currentHP;
    public float CurrentHP => currentHP;

    [SerializeField] private float deathTime = 1f;
    private bool isDead = false;

    private EnemyController enemyController;
    
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
        // Debug.Log($"남은 HP: {currentHP}");

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        enemyController.PlayDeathAnimation();
        StartCoroutine(DeathTerm());
    }

    private IEnumerator DeathTerm()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}

