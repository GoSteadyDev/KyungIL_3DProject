using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LazerTower : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")]
    [SerializeField] private TowerType TowerType;
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private float damagePerSec = 10f;
    [SerializeField] private float attackRange = 30f;
    
    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint; 
    [SerializeField] private GameObject Lazer;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite icon;

    private Transform targetTransform;
    private Animator animator;
    
    private bool isFiring = false;

    public string GetDisplayName() => "LaserTower Lv3B";
    public Sprite GetIcon() => icon;
    public string GetDescription() => $"직선형 범위 데미지\nDamage: {damagePerSec}";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerType GetTowerType() => TowerType;
    public int GetCurrentLevel() => currentLevel;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        FindTarget();

        if (targetTransform != null)
        {
            animator.SetTrigger("IsAttack");
            
            Lazer.SetActive(true);
            Lazer.GetComponent<Lazer>().Initialize(firePoint, targetTransform, damagePerSec);
        }
        else
        {
            Lazer.SetActive(false);
        }
    }
    
    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
