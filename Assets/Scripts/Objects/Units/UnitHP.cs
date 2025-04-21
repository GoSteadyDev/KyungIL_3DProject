using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHP : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    public float MaxHP { get; }
    private float currentHP;
    public float CurrentHP => currentHP;
    [SerializeField] private float deathTime = 0.5f;

    private bool isDead = false;
    private SwordMan swordMan;

    private void Awake()
    {
        swordMan = GetComponent<SwordMan>();
        
        currentHP = Mathf.Clamp( currentHP, 0, maxHP);
        currentHP = maxHP;
    }

    private void Update()
    {
        // float value = CurrentHP / MaxHP;
        // hpSlider.value = Mathf.Clamp01(value);
        //
        // Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 10f);
        // hpSlider.transform.position = screenPos;
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
        swordMan.enabled = false;
        StartCoroutine(DeathTerm());
    }

    private IEnumerator DeathTerm()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}