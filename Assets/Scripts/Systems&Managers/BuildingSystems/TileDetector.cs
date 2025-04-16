using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TileDetector : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private Camera mainCamera;
    
    private BuildingPointTile currentTile = null;
    private BuildingPointTile previousTile = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
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
}