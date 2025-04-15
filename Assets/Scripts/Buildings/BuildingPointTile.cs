using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPointTile : MonoBehaviour
{
    private MeshRenderer Renderer;
public bool active = false;
    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        Renderer.enabled = false;
    }

    public void TurnOnRenderer()
    {
        Renderer.enabled = true;
    }

    public void TurnOffRenderer()
    {
        Renderer.enabled = false;
    }
}
