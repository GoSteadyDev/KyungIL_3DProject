using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerBuilder : MonoBehaviour
{
       public static TowerBuilder Instance { get; private set; }
       
       [FormerlySerializedAs("objectDector")] [FormerlySerializedAs("tileDector")] [SerializeField] private TileDetector tileDetector;
       [SerializeField] private GameObject buildPointPrefab;
       [SerializeField] private GameObject towerPrefab;

       private void Awake()
       {
              Instance = this;
       }

       private void Update()
       { 
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

       public void OnBuildPointClicked(BuildingPoint buildingPoint)
       {
              // 위 데이터들을 고려해서 생성 되게 하는 로직 들어가기
              Instantiate(towerPrefab, buildingPoint.transform.position, Quaternion.identity);
              Destroy(buildingPoint.gameObject);
       }

       public void OnTowerSelected(GameObject towerPrefab, Vector3 position)
       {
              Instantiate(towerPrefab, position, Quaternion.identity);
       }
}
