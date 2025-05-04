using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Projectile, AreaProjectile, Beam, Direct }
public enum TowerType { Archer, Cannon, Mage }

public interface ITower : IClickable
{
    TowerType GetTowerType();
    int GetCurrentLevel();
}

[Serializable]
public struct AttackData
// 이걸 struct로 설계한 이유
// class -> 공유·변경 가능한 상태를 다룰 때
// struct -> 메서드 없이 순수 값 형태를 읽기 전용으로 다룰 때 좋음 (원본 유지)
// (내가 공격력 증가 타워를 만들었는데, 그럼 버프 범위 밖에 있는 타워도 공격력이 증가하게 되는 원인이 될 코드를 작성할 수 있다. target.data.Damage += 10; 뭐 이런식으로 설정하면)
// 예를 들어 TowerA, TowerB가 같은 TowerData를 공유하고 있다면:
// SetAttackData(towerData.attackData);를 두 타워가 동일하게 받는 순간, 한쪽에서 수정하면 다른 쪽 타워에도 영향 발생 가능
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
    [Tooltip("연속 발사할 횟수")]
    public int   burstCount;            
    [Tooltip("버스트 간 발사 간격(초)")]
    public float burstInterval;        
    
    [Tooltip("투사체 비행 시간 대비 적 선도 보정 계수")]
    public float overshootFactor;
    
    // Beam 전용 (Raycast 대신 OverlapBox)
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