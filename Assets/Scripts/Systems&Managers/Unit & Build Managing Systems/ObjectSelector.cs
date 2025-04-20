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
    string GetDisplayName();
    Sprite GetIcon();
    string GetDescription(); // 나중에 확장 가능
}

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RangeViewerController rangeViewer;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
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

                IHasInfoPanel infoTarget = hit.collider.GetComponent<IHasInfoPanel>();
                if (infoTarget != null)
                {
                    UIManager.Instance.ShowInfoPanel(infoTarget);
                }
                else
                {
                    UIManager.Instance.HideInfoPanel();
                }
            }

        }
    }
}

