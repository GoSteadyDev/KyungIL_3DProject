using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class TowerBuilder : MonoBehaviour
{
       public static TowerBuilder Instance { get; private set; }
       
       [SerializeField] private TileDetector tileDetector;
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
              GameObject buildPointGO = Instantiate(buildPointPrefab, tile.transform.position, Quaternion.identity);
              tile.PickedByPlayer();

              BuildingPoint point = buildPointGO.GetComponent<BuildingPoint>();
              point.Init(tile); // ✅ 생성된 BuildPoint에게 타일 정보 넘김
       }

       public bool BuildTower(GameObject towerPrefab, Vector3 position)
       {
              if (ResourceManager.Instance.TrySpendGold(buildMoney))
              {
                     Instantiate(towerPrefab, position, Quaternion.identity);
                     return true; // 성공
              }
              UIManager.Instance.ShowWarning("You don't have enough gold!");
              return false;
       }
}
