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

public enum TowerType
{
    Archer,
    Cannon,
    Mage
}

public interface ITower
{
    public interface IBranchingTower
    {
        string GetPathCode();
    }

    Transform GetTransform();
    TowerType GetTowerType();
    int GetCurrentLevel();
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

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HandleSelection(hit);
        }
    }

    private void HandleSelection(RaycastHit hit)
    {
        HandleRangeUI(hit);
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
            UIManager.Instance.ShowInfoPanel(infoTarget);
            HandleTowerPanel(infoTarget);
        }
        else
        {
            UIManager.Instance.HideInfoPanel();
            UIManager.Instance.HideAllTowerPanels();
        }
    }

    private void HandleTowerPanel(IHasInfoPanel infoTarget)
    {
        if (infoTarget is ITower tower)
        {
            currentSelectedTower = tower;
            Vector3 pos = tower.GetTransform().position;
            int level = tower.GetCurrentLevel();

            if (level == 1)
                UIManager.Instance.ShowTowerPanelByLevel(1, pos);
            else if (level == 2)
                UIManager.Instance.ShowTowerPanelByLevel(2, pos);
            else
                UIManager.Instance.ShowTowerPanelByLevel(3, pos);
        }
    }
}