using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using System.Collections.Generic;
using UnityEngine;

public class ObjectDector : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Camera mainCamera;
    private BuildingPointTile currentTile = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject buildPointPrefab;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            BuildingPointTile tile = hit.collider.GetComponent<BuildingPointTile>();

            if (tile != null)
            {
                if (tile != currentTile)
                {
                    // 이전 타일 비활성화
                    if (currentTile != null)
                        currentTile.TurnOffRenderer();

                    // 새 타일 활성화
                    currentTile = tile;
                    currentTile.TurnOnRenderer();
                }

                // 클릭 시 BuildPoint 생성
                if (Input.GetMouseButtonDown(0))
                {
                    CreateBuildingPoint(tile.transform.position);
                    tile.active = true;
                }
            }
            else
            {
                if (currentTile != null)
                {
                    currentTile.TurnOffRenderer();
                    currentTile = null;
                }
            }
           
        }
        else
        {
            if (currentTile != null)
            {
                currentTile.TurnOffRenderer();
                currentTile = null;
            }
        }
    }

    private void CreateBuildingPoint(Vector3 position)
    {
        Instantiate(buildPointPrefab, position, Quaternion.identity);
    }
}

