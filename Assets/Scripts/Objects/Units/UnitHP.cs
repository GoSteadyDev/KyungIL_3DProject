using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHP : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    public float MaxHP => maxHP;
    private float currentHP;
    public float CurrentHP => currentHP;
    [SerializeField] private float deathTime = 0.5f;

    private bool isDead = false;
    private UnitController unitController;

    private void Awake()
    {
        unitController = GetComponent<UnitController>();
        
        currentHP = Mathf.Clamp( currentHP, 0, maxHP);
        currentHP = maxHP;
    }
    
    public void TakeDamage(float amount)
    {
        Debug.Log(amount);
        if (isDead) return;

        currentHP -= amount;

        if (currentHP <= 0f)
        {
            Die();
        }
    }
    
    private void Die()
    {
        unitController.enabled = false;
        StartCoroutine(DeathTerm());
    }

    private IEnumerator DeathTerm()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}