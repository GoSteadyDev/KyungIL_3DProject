using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPoint : MonoBehaviour
{    
    private BuildingPointTile originTile;  // 자신이 생성된 타일
    
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))  // 안전하게 좌클릭 조건 확인
        {
            BuildingSystem.Instance.OpenBuildUI(this);
        }
    }
}