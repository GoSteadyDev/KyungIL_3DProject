using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPoint : MonoBehaviour
{    
    private BuildingPointTile originTile;  // 자신이 생성된 타일
    public BuildingPointTile OriginTile => originTile;
    // 이건 필요 없는 프로퍼티네? BuildingPoint라는 스크립트가 꼭 필요할까?, 마우스 클릭 인식만 기능하는 것 같다
    public void Init(BuildingPointTile tile)
    {
        originTile = tile;
    }
    
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))  // 안전하게 좌클릭 조건 확인
        {
            BuildingSystem.Instance.OpenBuildUI(this);
        }
    }
}