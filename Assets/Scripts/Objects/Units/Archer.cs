using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour, IHasRangeUI, IHasInfoPanel
{
    [Header("Attack Settings")] 
    [SerializeField] private float damage = 2f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("Info Settings")]
    [SerializeField] private Sprite icon;
    
    public string GetDisplayName() => "Archer";
    public Sprite GetIcon() => icon;
    // 프로퍼티 바꾸기 후순위 한번해보기
    // public Sprite GetIcon
    // {
    //     get => icon;
    //     set => icon = value;
    // }
    
    public string GetDescription() => "Damage : \n\nAttackRange : \n\nAttackSpeed : ";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;

    private Animator animator;
    
    private float nextAttackTime;
    private Transform targetTransform;
    private float targetdistance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        FindTarget();
        
        if (targetTransform != null)
        {
            Vector3 dir = targetTransform.position - transform.position;
            dir.y = 0;
            
            if (dir != Vector3.zero)
                transform.forward = dir.normalized;
            
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackSpeed;
                FireArrow();
            }
            targetdistance = Vector3.Distance(transform.position, targetTransform.position);
        }
        
        if (targetdistance > attackRange)
            targetTransform = null;
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    // 애니메이션 이벤트에서 호출
    public void FireArrow()
    {
        if (targetTransform == null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.SetTargetAndDamage(targetTransform, damage);
        
        StartAttackAnim();
    }
        
    public void StartAttackAnim()
    {
        animator.SetTrigger("IsAttack");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

