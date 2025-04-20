using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    [SerializeField] private TileDetector tileDetector;  // Inspector에서 연결
    [SerializeField] private GameObject previewObjectPrefab;
    private GameObject currentPreview;
    
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    private Camera mainCamera;
    private bool isActive = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void ActivatePreview()
    {
        if (currentPreview != null) Destroy(currentPreview);
        currentPreview = Instantiate(previewObjectPrefab);
        isActive = true;

        tileDetector.enabled = true; // ✅ TileDetector 켜기
        
        foreach (var tile in tileDetector.Tiles)
        {
            if (!tile.IsUsed)
                tile.ResetTile(); // collider.enabled = true
        }
    }

    public void DeactivatePreview()
    {
        if (currentPreview != null) Destroy(currentPreview);
        isActive = false;

        tileDetector.ClearTileHighlight(); // ✅ 타일 렌더 꺼주기
        tileDetector.enabled = false;
    }
    
    private void Update()
    {
        if (!isActive || currentPreview == null) return;

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
}
