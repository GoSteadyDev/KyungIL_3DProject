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
    
    public int GetTotalEnemyCount
    {
        get
        {
            int total = 0;
            
            foreach (var info in enemyList)
            {
                total += info.enemyCount;
            }
            return total;
        }
    }
}