using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTowerLv3 : MonoBehaviour, IHasRangeUI, IHasInfoPanel, ITower
{
    [Header("Tower Settings")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float slowRate = 0.25f;
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private float attackRange = 7.5f;
    [SerializeField] private float attackInterval = 1f;
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject slowEffectPrefab;
    [SerializeField] private ParticleSystem attackEffectPrefab;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite icon;

    private Animator animator;
    private List<EnemyController> detectedEnemies = new();
    private bool isFiring = false;

    public string GetDisplayName() => "SlowTower Lv3A";
    public Sprite GetIcon() => icon;
    public string GetDescription() => $"\nDamage: {damage} \nSlow: {slowRate * 100}% / {slowDuration}s";
    public float GetAttackRange() => attackRange;
    public Transform GetTransform() => transform;
    public TowerTemplate GetTowerTemplate() => towerTemplate;
    public int GetCurrentLevel() => currentLevel;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        detectedEnemies.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                detectedEnemies.Add(enemy);
            }
        }

        if (detectedEnemies.Count > 0 && !isFiring)
        {
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        isFiring = true;
        animator.SetTrigger("IsAttack");
        attackEffectPrefab.Play();
        
        yield return new WaitForSeconds(0.1f); // 약간의 연출 텀

        foreach (var enemy in detectedEnemies)
        {
            if (enemy == null) continue;

            enemy.ApplySlow(slowRate, slowDuration, slowEffectPrefab);

            CombatEvent combatEvent = new CombatEvent
            {
                Sender = this.gameObject,
                Receiver = enemy.gameObject,
                Damage = damage,
                HitPosition = enemy.transform.position,
                Collider = enemy.GetComponent<Collider>()
            };

            CombatSystem.Instance.AddCombatEvent(combatEvent);
        }

        yield return new WaitForSeconds(attackInterval);
        isFiring = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
