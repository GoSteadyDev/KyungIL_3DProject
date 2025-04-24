using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CannonTower : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")] 
    [SerializeField] private TowerType TowerType;
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float attackRange = 10f;
    
    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private float cannonSpeed = 30f;
    
    [Header("Visual Settings")]
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private Sprite icon;
    
    private Transform targetTransform;
    
    private float currentCooldown;
    
    public string GetDisplayName() => "CannonTower"; 
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerType GetTowerType() => TowerType;
    public int GetCurrentLevel() => currentLevel;
    
    void Update()
    {
        FindTarget();

        if (targetTransform != null)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                Fire();
                currentCooldown = attackSpeed;
            }
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    private void Fire()
    {
        fireEffect.Play();
        if (targetTransform == null) return;

        EnemyController enemy = targetTransform.GetComponent<EnemyController>();
        if (enemy == null) return;

        Vector3 enemyPos = enemy.GetCurrentPosition();
        Vector3 enemyVelocity = enemy.GetVelocity();

        float projectileSpeed = cannonSpeed;

        // ✅ 1. 미래 위치 예측
        Vector3 predictedPosition = PredictFuturePosition(enemyPos, enemyVelocity, firePoint.position, projectileSpeed);

        // ✅ 2. 예측 시간 재계산 (더 정확하게)
        float distance = Vector3.Distance(firePoint.position, predictedPosition);
        float timeToTarget = distance / projectileSpeed;

        // ✅ 3. 포탄 생성 및 목표 위치 전달
        GameObject ball = Instantiate(cannonPrefab, firePoint.position, Quaternion.identity);
        ball.GetComponent<Cannon>().SetTarget(predictedPosition, timeToTarget);
    }
    
    private Vector3 PredictFuturePosition(Vector3 enemyPos, Vector3 enemyVelocity, Vector3 shooterPos, float projectileSpeed)
    {
        Vector3 toEnemy = enemyPos - shooterPos;
        float distance = toEnemy.magnitude;
        float timeToTarget = distance / projectileSpeed;    // 타겟에 이동하는 시간
        float overshootFactor = 0.5f; // 1보다 작게 하면 조준을 짧게 함

        return enemyPos + enemyVelocity * (timeToTarget * overshootFactor);     // 적 위치 + 적 속도 * 타겟에 이동하는 시간
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

