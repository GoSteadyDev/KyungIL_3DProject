using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Template/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public TowerData data;      // SO: 스탯·비용·pathCode 포함
        public GameObject prefab;   // 각 타입·레벨·분기별 Prefab
    }

    public List<Entry> entries;

    // 기본 조회: 타입, 레벨, 분기(PathCode)까지 완벽 매칭
    public Entry GetEntry(TowerType type, int level, string pathCode)
    {
        return entries.First(e =>
            e.data.towerType == type
            && e.data.level == level
            && e.data.pathCode == pathCode);
    }

    // 주어진 타입-레벨에 가능한 모든 분기(PathCode) 목록 조회
    // UI에서 업그레이드 경로 버튼 생성 등에 활용
    public IEnumerable<Entry> GetEntries(TowerType type, int level)
    {
        return entries.Where(e =>
            e.data.towerType == type
            && e.data.level == level);
    }
}