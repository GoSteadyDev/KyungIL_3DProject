using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IDamageable, IHasInfoPanel
{
    [Header("Base Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private GameObject hpViewerPrefab;
    
    [Header("Visual Settings")]
    [SerializeField] private GameObject DestroyEffect;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private Sprite icon;
    
    MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    
    private float currentHP;

    public string GetDescription() => $"HP : {currentHP}/{maxHP}\nDefend the castle\nfrom attacking\nenemies!";
    public string GetDisplayName() => "Castle"; 
    public Sprite GetIcon() => icon; 
    public Transform GetTransform() => transform;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        currentHP = maxHP;
    }
    
    private void Start()
    {
        HPViewerSpawner.CreateHPViewer(this, this.transform, hpViewerPrefab);
    }

    public void TakeDamage(float damage)
    {
        NotificationService.Notify("Your Base is under attack!");
        currentHP -= damage;

        if (currentHP >= maxHP * 0.6f)
        {
            meshFilter.mesh = meshes[0];
        }
        else if (currentHP < maxHP * 0.6f && currentHP >= maxHP * 0.3f)
        {
            meshFilter.mesh = meshes[1];
        }
        else if (currentHP < maxHP * 0.3f && currentHP >= 0)
        {
            meshFilter.mesh = meshes[2];
        }
        else
        {
            DestroyEffect.SetActive(true);
            meshRenderer.enabled = false;
            StartCoroutine(HandleCastleDestruction());
        }
    }
    
    private IEnumerator HandleCastleDestruction()
    {
        yield return new WaitForSecondsRealtime(2f);

        // 실제 파괴 처리
        Destroy(gameObject);
        GameManager.Instance.OnCastleDestroyed();
    }
}