using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LazerTower : MonoBehaviour, ISelectable, IHasInfoPanel, ITower
{
    [Header("Attack Settings")]
    [SerializeField] private float damagePerSec = 10f;
    [SerializeField] private float attackRange = 30f;
    [SerializeField] private GameObject LazerEffect;
    [SerializeField] private Transform firePoint; 
    
    [Header("UI Info")]
    [SerializeField] private Sprite icon;
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private int currentLevel = 0;

    private Animator animator;
    private bool isFiring = false;
    private Transform targetTransform;

    public string GetDisplayName() => "LaserTower Lv3B";
    public Sprite GetIcon() => icon;
    public string GetDescription() => $"직선형 범위 데미지\nDamage: {damagePerSec}";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerTemplate GetTowerTemplate() => towerTemplate;
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
            
            LazerEffect.SetActive(true);
            LazerEffect.GetComponent<Lazer>().Initialize(firePoint, targetTransform, damagePerSec);
        }
        else
        {
            LazerEffect.SetActive(false);
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
