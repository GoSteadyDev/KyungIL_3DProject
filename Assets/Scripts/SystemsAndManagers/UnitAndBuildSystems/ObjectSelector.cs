using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IHasRangeUI
{
    float GetAttackRange();
    Transform GetTransform();
}

public interface IHasInfoPanel
{
    Sprite GetIcon();
    string GetDisplayName();
    string GetDescription(); // 나중에 확장 가능
}

public class ObjectSelector : MonoBehaviour
{
    public static ObjectSelector Instance;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RangeViewerController rangeViewer;

    private ITower currentSelectedTower;
    public ITower CurrentSelectedTower => currentSelectedTower;
    
    private void Awake()
    {
        Instance = this;
        
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;

        // 0) 이전 모든 UI 닫기
        UIManager.Instance.HideAllTowerPanels();

        // 1) 범위(Range) 표시
        HandleRangeUI(hit);

        // 2) Panel/Info 표시
        HandleInfoPanel(hit);
    }

    private void HandleRangeUI(RaycastHit hit)
    {
        IHasRangeUI hasRangeUI = hit.collider.GetComponent<IHasRangeUI>();

        if (hasRangeUI != null)
        {
            rangeViewer.SetTarget(hasRangeUI.GetTransform(), hasRangeUI.GetAttackRange());
        }
        else
        {
            rangeViewer.Clear();
        }
    }
    
    private void HandleInfoPanel(RaycastHit hit)
    {
        IHasInfoPanel infoTarget = hit.collider.GetComponent<IHasInfoPanel>();

        if (infoTarget != null)
        {
            if (infoTarget is ITower tower)
            {
                HandleTowerPanel(infoTarget);
                UIManager.Instance.ShowTowerInfoPanel(infoTarget, tower.GetTransform());
            }
            else
            {
                UIManager.Instance.ShowUnitInfoPanel(infoTarget);
            }
        }
        else
        {
            UIManager.Instance.HideAllTowerPanels();
        }
    }
    
    private void HandleTowerPanel(IHasInfoPanel infoTarget)
    {
        if (infoTarget is ITower tower)
        {
            currentSelectedTower = tower;
            int level = tower.GetCurrentLevel();
            
            UIManager.Instance.ShowTowerPanelByLevel(level, tower.GetTransform());
        }
    }
}