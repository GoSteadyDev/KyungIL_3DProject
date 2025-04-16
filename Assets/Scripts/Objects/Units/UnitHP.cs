using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHP : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private float deathTime = 1f;
    private float currentHP;
    public float CurrentHP => currentHP;
    
    private bool isDead = false;
    private SwordMan swordMan;

    private void Awake()
    {
        currentHP = Mathf.Clamp( currentHP, 0, maxHP);
        currentHP = maxHP;
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        Debug.Log($"남은 HP: {currentHP}");

        if (currentHP <= 0f)
        {
            
        }
    }
}