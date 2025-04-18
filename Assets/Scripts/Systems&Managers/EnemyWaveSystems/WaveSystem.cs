using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public IEnumerator PlayWave(WaveData waveData, EnemySpawner spawner)
    {
        foreach (var spawnInfo in waveData.enemyList)
        {
            for (int i = 0; i < spawnInfo.count; i++)
            {
                spawner.Spawn(spawnInfo.enemyData);
                yield return new WaitForSeconds(waveData.spawnInterval);
            }
        }
    }
}