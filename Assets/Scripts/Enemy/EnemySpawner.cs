using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private EnemyData enemyData;

    private Dictionary<string, GameObject> enemyDic = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    [SerializeField] private float spawnInterval = 2f;
    private float timer;
    
    private void Awake()
    {
        if (!enemyDic.ContainsKey(enemyData.EnemyName))
        {
            enemyDic.Add(enemyData.EnemyName, enemyData.EnemyPrefab);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn("Tree");
        }
    }
    
    public void Spawn(string enemyName)
    {
        if (!enemyDic.ContainsKey(enemyName)) return;

        GameObject enemy = Instantiate(enemyDic[enemyName], spawnPoint.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        
        // WayPoint 정보 전달
        EnemyController controller = enemy.GetComponent<EnemyController>();
        
        if (controller != null)
        {
            controller.SetWayPoints(wayPoints);
        }
    }
}