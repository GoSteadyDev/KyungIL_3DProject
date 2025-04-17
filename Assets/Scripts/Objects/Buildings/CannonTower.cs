using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : MonoBehaviour
{
    [Header("Attack Settings")] 
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private Transform firePoint;
    
    private Transform targetTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();

        Debug.Log($"{targetTransform.name}");
        
        if (targetTransform != null)
        {
            Fire();
        }
    }
    
    private void FindTarget()
    {   
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        targetTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    private void Fire()
    {
        var tempTime = Time.deltaTime;

        while (attackSpeed > tempTime)
        {
            GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, Quaternion.identity);
            CannonBall projectile = cannonBall.GetComponent<CannonBall>();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
