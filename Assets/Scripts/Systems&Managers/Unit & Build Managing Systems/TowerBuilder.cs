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
              if (tile.IsUsed) return; // ✅ 이미 사용된 타일은 무시
              
              GameObject buildPointGO = Instantiate(buildPointPrefab, tile.transform.position, Quaternion.identity);
              tile.PickedByPlayer();

              BuildingPoint point = buildPointGO.GetComponent<BuildingPoint>();
              point.Init(tile); // ✅ 생성된 BuildPoint에게 타일 정보 넘김
       }

       public bool BuildTower(GameObject towerPrefab, Vector3 position)
       {
              if (ResourceManager.Instance.TrySpendGold(buildMoney))
              {
                     GameObject tower = Instantiate(towerPrefab, position, Quaternion.identity);

                     // ✅ 해당 위치의 타일 찾아서 사용 처리
                     Ray ray = new Ray(position + Vector3.up * 10, Vector3.down);
                     if (Physics.Raycast(ray, out RaycastHit hit, 20f))
                     {
                            var tile = hit.collider.GetComponent<BuildingPointTile>();
                            if (tile != null)
                                   tile.SetUsed(); // 타워 설치된 경우만 사용 상태로!
                     }
                     return true;
              }

              UIManager.Instance.ShowWarning("You don't have enough gold!");
              return false;
       }

}
