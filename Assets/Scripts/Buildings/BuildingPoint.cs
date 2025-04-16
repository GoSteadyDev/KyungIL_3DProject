using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPoint : MonoBehaviour
{
    public void OnMouseDown()
    {
        // TowerBuilder.Instance.OnBuildPointClicked(this);
        
        // 클릭하면 UI 호출되도록
        BuildingSystem.Instance.OpenBuildUI(this);
    }
}