using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waveDatas;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private WaveSystem waveSystem;

    private int currentWaveIndex = 0;

    public void StartNextWave()
    {
        if (currentWaveIndex >= waveDatas.Count) return;

        StartCoroutine(waveSystem.PlayWave(waveDatas[currentWaveIndex], spawner));
        currentWaveIndex++;
    }
}
