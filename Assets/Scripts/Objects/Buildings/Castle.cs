using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IDamageable, IHasInfoPanel
{
    [SerializeField] private GameObject hpViewerPrefab;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private GameObject DestroyEffect;
    [SerializeField] private Sprite icon;
    
    public string GetDisplayName() => "Castle"; 
    public Sprite GetIcon() => icon; 
    public string GetDescription() => $"HP : {currentHP}/{maxHP}";
    
    private float maxHP = 100f;
    public float MaxHP => maxHP;

    private float currentHP;
    public float CurrentHP => currentHP;


    private MeshFilter meshFilter;
    
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        currentHP = maxHP;
    }
    
    private void Start()
    {
        HPViewerSpawner.CreateHPViewer(this, this.transform, hpViewerPrefab);
    }

    public void TakeDamage(float damage)
    {
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
            GameManager.Instance.OnCastleDestroyed();
            Destroy(gameObject);
        }
    }
}

