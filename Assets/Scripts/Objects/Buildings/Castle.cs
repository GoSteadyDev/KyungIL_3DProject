using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject hpViewerPrefab;
    [SerializeField] private Mesh[] meshes;
    
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
        Debug.Log($"왕국 HP : {currentHP}");

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
            Destroy(gameObject);
        }
    }
}

