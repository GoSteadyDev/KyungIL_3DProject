using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{   
    [Header("Unit Transform Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform startPoint; // waypoint07
    [SerializeField] private Transform endPoint;   // warfGate

    [Header("Visuals")] 
    [SerializeField] private GameObject SwordManPrefab;
    [SerializeField] private GameObject SpearManPrefab;
    [SerializeField] private ParticleSystem spawnEffect;
    [SerializeField] private GameObject hpViewerPrefab;
    
    public void SpawnSwordMan()
    {
        if (ResourceManager.Instance.TrySpendGold(5) == false) return;
        
        spawnEffect.Play();
        NotificationService.Notify("Swordman deployed. Holding the line.");
        GameObject unit = Instantiate(SwordManPrefab, spawnPoint.position, Quaternion.identity);
        MinimapBlipManager.Instance.RegisterTarget(unit.transform, Color.cyan);

        var controller = unit.GetComponent<UnitController>();
        
        if (controller != null)
        {
            controller.SetWayPath(startPoint, endPoint);
        }

        var unitHP = unit.GetComponent<UnitHP>();
        HPViewerSpawner.CreateHPViewer(unitHP, unit.transform, hpViewerPrefab);
    }
    
    public void SpawnSpearMan()
    {
        if (ResourceManager.Instance.TrySpendGold(7) == false) return;
        
        spawnEffect.Play();
        NotificationService.Notify("Spearman unleashed. Aim to kill.");
        GameObject unit = Instantiate(SpearManPrefab, spawnPoint.position, Quaternion.identity);
        MinimapBlipManager.Instance.RegisterTarget(unit.transform, Color.cyan);
        
        var controller = unit.GetComponent<UnitController>();
        
        if (controller != null)
        {
            controller.SetWayPath(startPoint, endPoint);
        }

        var unitHP = unit.GetComponent<UnitHP>();
        HPViewerSpawner.CreateHPViewer(unitHP, unit.transform, hpViewerPrefab);
    }
}