using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPoint : MonoBehaviour
{
    public void OnMouseDown()
    {
        BuildingSystem.Instance.OpenBuildUI(this);
    }
}