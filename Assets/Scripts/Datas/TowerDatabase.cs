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
    // First : 조건에 딱 하나만 필요한 경우 찾자마자 반환하고 끝냄
    public Entry GetEntry(TowerType type, int level, string pathCode)
    {
        return entries.First(e => e.data.towerType == type && e.data.level == level
            && e.data.pathCode == pathCode);
    }

    // 주어진 타입-레벨에 가능한 모든 분기(PathCode) 목록 조회
    // UI에서 업그레이드 경로 버튼 생성 등에 활용
    // Where : 조건에 맞는 모든 항목을 찾음, 0개, 1개, 여러 개 가능
    // GetEntries()는 "동일한 타입 + 레벨"의 여러 분기(PathCode)가 존재할 수 있으므로 Where()로 여러 개를 반환하는 방식
    
    public List<Entry> GetEntryByLevel(TowerType type, int level)
    {
        return entries.Where(e => e.data.towerType == type && e.data.level == level).ToList();
    }
}