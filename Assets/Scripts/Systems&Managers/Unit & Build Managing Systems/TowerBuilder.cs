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

       public bool BuildTower(TowerTemplate template, Vector3 position)
       {
              if (!ResourceManager.Instance.TrySpendGold(template.cost))
              {
                     return false;
              }
              
              // BuildTower 내부
              var towerGO = Instantiate(template.towerPrefab, position, Quaternion.identity);
              // ITower 구현체를 GetComponent로 가져오고 등록
              var towerComp = towerGO.GetComponent<ITower>();
              BuildingSystem.Instance.Register(towerComp);
              
              // 위치한 타일에 설치됨 표시
              Ray ray = new Ray(position + Vector3.up * 10, Vector3.down);
              if (Physics.Raycast(ray, out RaycastHit hit, 20f))
              {
                     var tile = hit.collider.GetComponent<BuildingPointTile>();
                     tile?.SetUsed();
              }

              return true;
       }

       public bool UpgradeTower(ITower oldTower, GameObject newTowerPrefab, Vector3 position, Quaternion rotation)
       {
              // UpgradeTower 내부
              var oldMb = oldTower as MonoBehaviour;
              BuildingSystem.Instance.Unregister(oldTower);
              Destroy(oldMb.gameObject);

              var newGO = Instantiate(newTowerPrefab, position, rotation);
              var newTowerComp = newGO.GetComponent<ITower>();
              BuildingSystem.Instance.Register(newTowerComp);

              // 기존 타워 제거
              // if (oldTower is MonoBehaviour mb)
              // {
              //        Destroy(mb.gameObject);
              // }

              // 타일 상태는 그대로 유지되도록 처리 필요 시 여기에 추가
              return true;
       }
}