using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Projectile, AreaProjectile, Beam, Direct }
public enum TowerType { Archer, Cannon, Mage }

public interface ITower
{
    Transform GetTransform();
    TowerType GetTowerType();
    int GetCurrentLevel();
}

[Serializable]
public struct AttackData
{
    public AttackType attackType;

    // Projectile/AreaProjectile 공통
    public GameObject projectilePrefab;   // 화살, 캐논볼, 미사일 프리팹
    public float projectileSpeed;         // 투사체 속도

    // ◀— AreaProjectile 전용(캐논 폭발)
    [Tooltip("폭발 이펙트(파티클 등)")]
    public GameObject areaEffectPrefab; 
    [Tooltip("폭발 반경")]
    public float     areaRadius;
    
    // ◀— Burst(멀티샷) 전용
    [Tooltip("한 번에 연속 발사할 횟수")]
    public int   burstCount;            
    [Tooltip("버스트 간 발사 간격(초)")]
    public float burstInterval;        
    
    [Tooltip("투사체 비행 시간 대비 적 선도 보정 계수")]
    public float overshootFactor;
    
    // Beam 전용
    // ◀— Beam 전용 (Raycast 대신 OverlapBox)
    [Tooltip("레이저 충돌 박스 크기(절반 크기)")]
    public Vector3 beamBoxHalfExtents;
    [Tooltip("레이저 데미지 처리 간격")]
    public float beamInterval;

    // Direct 전용 (SlowTower)
    public GameObject slowEffectPrefab;
    public float slowRate;                // 감속 비율
    public float slowDuration;            // 감속 지속시간
}

[CreateAssetMenu(menuName="Template/TowerData", fileName="NewTowerData")]
public class TowerData : ScriptableObject
{
    public TowerType towerType;
    public int level;
    public int buildCost, upgradeCost, sellPrice;
    public string pathCode;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    // 기본 스탯
    public float damage;
    public float attackSpeed;
    public float attackRange;

    // 공격 로직
    public AttackData attackData;
    
    [Header("Upgrade Paths")]
    public List<TowerData> nextUpgrades; // References to next-level or branch TowerData assets
}