using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Transform Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private Transform castleTransform;
    [SerializeField] private EnemyData enemyData;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem spawnEffect;
    [SerializeField] private GameObject hpViewerPrefab;
    
    private Dictionary<string, GameObject> enemyDic = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    
    public void Spawn(EnemyData data)
    {
        if (!enemyDic.ContainsKey(data.EnemyName))
        {
            enemyDic.Add(data.EnemyName, data.EnemyPrefab);
        }

        GameObject enemy = Instantiate(data.EnemyPrefab, spawnPoint.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        var controller = enemy.GetComponent<EnemyController>();
        
        if (controller != null)
        {
            controller.SetWayPoints(wayPoints);
            controller.SetCastleTarget(castleTransform);
        }

        var enemyHP = enemy.GetComponent<EnemyHP>();
        HPViewerSpawner.CreateHPViewer(enemyHP, enemy.transform, hpViewerPrefab);

        spawnEffect.Play();
    }
    
    public bool HasAliveEnemies()
    {
        // 리스트에 살아있는 적이 하나라도 있으면 true
        spawnedEnemies.RemoveAll(enemy => enemy == null); // null 정리
        return spawnedEnemies.Count > 0;
    }
    
    public int GetAliveEnemyCount()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count;
    }

}