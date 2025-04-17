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

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private ParticleSystem spawnEffect;
    [SerializeField] private GameObject hpViewerPrefab;
    
    private Dictionary<string, GameObject> enemyDic = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
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
        if (spawnedEnemies.Count >= spawnCount) return;
        
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval )
        {
            timer = 0f;
            Spawn("Tree");
        }
    }
    
    public void Spawn(string enemyName)
    {
        spawnEffect.Play();
        
        if (!enemyDic.ContainsKey(enemyName))
        {
            Debug.Log("확인1");
            return;
        }

        GameObject enemy = Instantiate(enemyDic[enemyName], spawnPoint.position, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        
        EnemyController controller = enemy.GetComponent<EnemyController>();
        
        if (controller != null)
        {
            controller.SetWayPoints(wayPoints);
            controller.SetCastleTarget(castleTransform);
        }
        
        EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();
        HPViewerSpawner.CreateHPViewer(enemyHP, enemy.transform, hpViewerPrefab);
    }
}