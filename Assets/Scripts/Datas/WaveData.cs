using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public EnemyData enemyData;
        public int enemyCount;
    }

    public float spawnInterval = 1f;
    public List<EnemySpawnInfo> enemyList;
}