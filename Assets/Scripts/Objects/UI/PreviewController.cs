using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TileDetector tileDetector;  // Inspector에서 연결
    [SerializeField] private GameObject previewObjectPrefab;
    
    [Header("Preview Settings")]
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    private GameObject currentPreview;
    private Camera mainCamera;
    
    private bool isActive = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isActive == false || currentPreview == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 snappedPos = hit.collider.transform.position;
            currentPreview.transform.position = snappedPos;

            var tile = hit.collider.GetComponent<BuildingPointTile>();
            var renderer = currentPreview.GetComponent<Renderer>();

            if (tile != null)
            {
                renderer.material = validMaterial;
                
                if (Input.GetMouseButtonDown(0))
                {
                    TowerBuilder.Instance.OnTileClicked(tile);
                    DeactivatePreview();
                }
            }
            else
            {
                renderer.material = invalidMaterial;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeactivatePreview(); // 우클릭 시 취소
        }
    }
    
    public void ActivatePreview()
    {
        if (currentPreview != null) Destroy(currentPreview);
        currentPreview = Instantiate(previewObjectPrefab);
        
        isActive = true;

        tileDetector.enabled = true; // ✅ TileDetector 켜기
        
        // 여기에서 타일을 리셋해주기 때문에 굳이 BuildingPoint에서 Init 해주는 메서드는 필요 없는듯함.
        foreach (var tile in tileDetector.Tiles)
        {
            if (tile.IsUsed == false)
                tile.ResetTile(); // collider.enabled = true
        }
    }

    public void DeactivatePreview()
    {
        if (currentPreview != null) Destroy(currentPreview);
        isActive = false;

        tileDetector.ClearTileHighlight(); // 타일 렌더 꺼주기
        tileDetector.enabled = false;
    }
}
