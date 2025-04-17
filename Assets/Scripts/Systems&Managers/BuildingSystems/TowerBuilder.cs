using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class TowerBuilder : MonoBehaviour
{
       public static TowerBuilder Instance { get; private set; }
       
       [FormerlySerializedAs("objectDector")] [FormerlySerializedAs("tileDector")] [SerializeField] private TileDetector tileDetector;
       [SerializeField] private GameObject buildPointPrefab;
       [SerializeField] private GameObject towerPrefab;
       [SerializeField] private int buildMoney;

       private void Awake()
       {
              Instance = this;
       }

       private void Update()
       { 
              if (EventSystem.current.IsPointerOverGameObject()) return;
              
              if (Input.GetMouseButtonDown(0))
              {
                     BuildingPointTile tile = tileDetector.GetTileUnderMouse();
                     
                     if (tile != null)
                     {
                            OnTileClicked(tile);
                     }
              }
       }

       public void OnTileClicked(BuildingPointTile tile)
       {
              Instantiate(buildPointPrefab, tile.transform.position, Quaternion.identity);
              tile.PickedByPlayer();
       }

       public void BuildTower(GameObject towerPrefab, Vector3 position)
       {
              if (ResourceManager.Instance.TrySpendGold(buildMoney))
              {
                  Instantiate(towerPrefab, position, Quaternion.identity);
              }
       }
}
