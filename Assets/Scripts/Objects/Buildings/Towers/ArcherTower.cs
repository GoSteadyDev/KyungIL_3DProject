using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : BaseTower
{
    private List<Archer> archerUnits;

    private void Awake()
    {
        // 자식 Archer 컴포넌트를 항상 자동 수집
        archerUnits = new List<Archer>(GetComponentsInChildren<Archer>());
    }

    protected override void RefreshStats()
    {
        // 멀티샷 타워: 전체 데미지를 유지하되, 유닛별로 분배
        float perUnitDamage = data.damage;
        if (archerUnits.Count > 1)
            perUnitDamage = data.damage / archerUnits.Count;

        // 모든 아처 유닛에 데이터 및 분배된 스탯 주입
        foreach (var unit in archerUnits)
        {
            unit.SetAttackData(data.attackData);
            unit.SetStats(perUnitDamage, data.attackSpeed, data.attackRange);
        }
    }

    protected override Transform FindTarget()
    {
        // 여러 유닛 중 가장 먼저 타겟을 찾은 유닛의 타겟을 반환
        // 여기서 반환하는 target과 무관하게 BaseTower의 Update를 돌리기 위한 용도임
        foreach (var unit in archerUnits)
        {
            var target = unit.FindTarget();
            if (target != null)
                return target;
        }
        return null;
    }

    protected override void Attack(Transform dummy)
    {
        // 각 유닛이 자신만의 firePoint에서 독립적으로 타겟 검색 및 공격
        foreach (var unit in archerUnits)
        {
            var target = unit.FindTarget();
            if (target != null)
                unit.Attack(target);
        }
    }

    // 멀티 유닛 일 때만 유닛 수 표시
    public override string GetDescription()
    {
        if (archerUnits.Count > 1)
        {
            float totalDPS = data.damage * data.attackSpeed;
            return $"\nDamage(per shot): {data.damage / archerUnits.Count} \nUnits: {archerUnits.Count}, " +
                   $"\nAttackSpeed: {data.attackSpeed}, \nTotal DPS: {totalDPS}";
        }
        // 기본 설명 (BaseTower나 SO 버전)
        return base.GetDescription();
    }
}