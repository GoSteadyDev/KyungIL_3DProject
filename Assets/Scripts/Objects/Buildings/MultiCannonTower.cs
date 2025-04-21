using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCannonTower : MonoBehaviour, ISelectable, IHasInfoPanel, ITower
{
    [Header("Settings")]
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private float cannonBallSpeed = 30f;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private Sprite icon;
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private int currentLevel = 0;

    public string GetDisplayName() => "CannonTower Lv3B";
    public Sprite GetIcon() => icon;
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerTemplate GetTowerTemplate() => towerTemplate;
    public int GetCurrentLevel() => currentLevel;

    private Transform targetTransform;
    private float currentCooldown;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (targetTransform != null)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                StartCoroutine(FireBurst()); // 3연속 발사
                currentCooldown = attackSpeed;
            }
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    private IEnumerator FireBurst()
    {
        fireEffect.Play();
        animator.SetTrigger("IsAttack");

        if (targetTransform == null) yield break;

        Vector3 enemyPos = targetTransform.position;
        Vector3 enemyVelocity = Vector3.zero;

        if (targetTransform.TryGetComponent(out EnemyController enemy))
        {
            enemyPos = enemy.GetCurrentPosition();
            enemyVelocity = enemy.GetVelocity();
        }

        float projectileSpeed = cannonBallSpeed;
        Vector3 predictedPosition = PredictFuturePosition(enemyPos, enemyVelocity, transform.position, projectileSpeed);
        float timeToTarget = Vector3.Distance(transform.position, predictedPosition) / projectileSpeed;

        for (int i = 0; i < firePoints.Length; i++)
        {
            GameObject ball = Instantiate(cannonBallPrefab, firePoints[i].position, Quaternion.identity);
            ball.GetComponent<Cannon>().SetTarget(predictedPosition, timeToTarget);

            yield return new WaitForSeconds(0.2f); // 연사 간격
        }
    }

    private Vector3 PredictFuturePosition(Vector3 enemyPos, Vector3 enemyVelocity, Vector3 shooterPos, float projectileSpeed)
    {
        Vector3 toEnemy = enemyPos - shooterPos;
        float distance = toEnemy.magnitude;
        float timeToTarget = distance / projectileSpeed;
        float overshootFactor = 0.85f;

        return enemyPos + enemyVelocity * (timeToTarget * overshootFactor);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
