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

                    // ✅ 추가: 타워일 경우에만 World UI 호출
                    if (infoTarget is ITower tower)
                    {
                        currentSelectedTower = tower;
                        Vector3 pos = tower.GetTransform().position;

                        int level = tower.GetCurrentLevel();

                        // Final Upgrade 선택 분기
                        if (level == 0)
                        {
                            UIManager.Instance.ShowTowerLv1Panel(pos);
                        }
                        // 최종 레벨인 경우 → Sell 전용 패널
                        else if (level == 1)
                        {
                            UIManager.Instance.ShowTowerLv2Panel(pos);
                        }
                        else
                        {
                            UIManager.Instance.ShowTowerLv3Panel(pos);
                        }
                    }

                }
                else
                {
                    UIManager.Instance.HideInfoPanel();
                    UIManager.Instance.HideTowerLv1Panel();
                    
                }
            }

        }
    }
}

