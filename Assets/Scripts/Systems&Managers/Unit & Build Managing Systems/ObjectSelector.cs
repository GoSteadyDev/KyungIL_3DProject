using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ISelectable
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

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HandleSelection(hit);
        }
    }

    private void HandleSelection(RaycastHit hit)
    {
        HandleSelectable(hit);
        HandleInfoPanel(hit);
    }

    private void HandleSelectable(RaycastHit hit)
    {
        ISelectable selectable = hit.collider.GetComponent<ISelectable>();

        if (selectable != null)
        {
            rangeViewer.SetTarget(selectable.GetTransform(), selectable.GetAttackRange());
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
            UIManager.Instance.HideTowerLv1Panel();
        }
    }

    private void HandleTowerPanel(IHasInfoPanel infoTarget)
    {
        if (infoTarget is ITower tower)
        {
            currentSelectedTower = tower;
            Vector3 pos = tower.GetTransform().position;
            int level = tower.GetCurrentLevel();

            if (level == 0)
                UIManager.Instance.ShowTowerLv1Panel(pos);
            else if (level == 1)
                UIManager.Instance.ShowTowerLv2Panel(pos);
            else
                UIManager.Instance.ShowTowerLv3Panel(pos);
        }
    }
}