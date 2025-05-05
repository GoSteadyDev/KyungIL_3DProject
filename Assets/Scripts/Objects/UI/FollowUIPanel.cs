using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// TowerCreateUI 패널 제어 스크립트
[RequireComponent(typeof(RectTransform))]
public class FollowUIPanel : MonoBehaviour
{
    [SerializeField] private Camera uiCamera; // UICamera를 Inspector에서 연결
    [SerializeField] private Canvas parentCanvas;
    
    private Camera cam;
    private RectTransform rectTransform;
    private Transform target;
    private Vector3 worldOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Transform target, Vector3 fixedWorldOffset)
    {
        this.target = target;
        this.worldOffset = fixedWorldOffset;

        gameObject.SetActive(true);
        UpdatePosition();
    }
    
    void LateUpdate()
    {
        if (target != null)
            UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position + worldOffset);
        Vector2 localPoint;
        
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();

        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, uiCamera, out localPoint);

        // ScreenPointToLocalPointInRectangle : 스크린 픽셀 좌표를 Canvas 기준 RectTransform의 로컬 좌표로 바꿔줌.
        // 다만 bool 값으로 해주는 이유는 좌표 변환이란게 꼭 성공하는 것은 아니기 때문
        // 여기 out -> bool 반환해주면서 + localpoint 바꿔주기 위해 써주는 문법임
        
        if (success)
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }
    public void Close()
    {
        target = null;
        gameObject.SetActive(false);
    }
}
