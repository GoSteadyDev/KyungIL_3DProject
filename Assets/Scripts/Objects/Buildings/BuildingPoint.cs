using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BuildingPoint : MonoBehaviour
{    
    private BuildingPointTile originTile;  // 자신이 생성된 타일

    public void Init(BuildingPointTile tile)
    {
        originTile = tile;
    }

    private void Update()
    {
        // 마우스 우클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            // 이 오브젝트 위를 클릭한 경우만
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    // ✅ 타일 복구
                    if (originTile != null)
                        originTile.ResetTile();
                    
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnMouseDown()
    {
        // 좌클릭일 경우 → UI 열기
        if (Input.GetMouseButtonDown(0))  // 안전하게 좌클릭 조건 확인
        {
            BuildingSystem.Instance.OpenBuildUI(this);
        }
    }
}
