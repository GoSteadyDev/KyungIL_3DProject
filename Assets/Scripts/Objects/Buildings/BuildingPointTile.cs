using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPointTile : MonoBehaviour
{
    private MeshRenderer Renderer;
    private Collider collider;
    
    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        Renderer.enabled = false;
        
        collider = GetComponent<Collider>();
    }

    public void TurnOnRenderer()
    {
        Renderer.enabled = true;
    }

    public void TurnOffRenderer()
    {
        Renderer.enabled = false;
    }

    public void PickedByPlayer()
    {
        collider.enabled = false;
    }
    
    public void ResetTile()
    {
        collider.enabled = true;
    }

}