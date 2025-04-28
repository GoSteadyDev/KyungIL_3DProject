using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
public class TileDetector : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Camera mainCamera;
    
    private BuildingPointTile currentTile = null;
    private BuildingPointTile previousTile = null;

    [SerializeField] private List<BuildingPointTile> allTiles = new List<BuildingPointTile>();
    public List<BuildingPointTile> Tiles => allTiles;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    // Inspector에서 수동 연결하거나 Start에서 자동 할당
    private void Start()
    {
        if (allTiles.Count == 0)
        {
            allTiles.AddRange(FindObjectsOfType<BuildingPointTile>());
        }
    }
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        DetectTileUnderMouse();
        DetectTileEffect();
    }

    private void DetectTileUnderMouse()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentTile = hit.collider.GetComponent<BuildingPointTile>();
        }
        else
        {
            currentTile = null;
        }
    }

    private void DetectTileEffect()
    {
        if (previousTile != null && previousTile != currentTile)
            previousTile.TurnOffRenderer();

        if (currentTile != null && currentTile != previousTile)
            currentTile.TurnOnRenderer();

        previousTile = currentTile;
    }
    
    public BuildingPointTile GetTileUnderMouse()
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<BuildingPointTile>();
        }
        return null;
    }
    
    public void ClearTileHighlight()
    {
        if (previousTile != null)
        {
            previousTile.TurnOffRenderer();
            previousTile = null;
        }
    }

}
