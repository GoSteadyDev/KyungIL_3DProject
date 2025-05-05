using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 미니맵 클릭 시 카메라 이동
public class MinimapClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("미니맵 전용 카메라(Orthographic)")]
    [SerializeField] private Camera minimapCamera;
    [Tooltip("메인 카메라 제어 스크립트")]
    [SerializeField] private CameraControlSystem cameraController;
    [Tooltip("지상 평면 Y 좌표")]
    [SerializeField] private float groundY;

    // RawImage의 RectTransform
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1) RawImage 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            ) == false)
            return;
        
        // pressEventCamera란?
        // PointerEventData.pressEventCamera,
        // 이건 PointerEventData에 포함된 “이 이벤트가 발생한 카메라” 정보를 담고 있어.
        // GraphicRaycaster가 이벤트를 쏠 때 사용한 카메라야.
        // 클릭 시점에 UI를 “누가 보고 있었는지”에 해당하는 카메라.

        // 2) 로컬 → 0~1 UV 좌표로 변환
        Vector2 size   = rectTransform.rect.size;
        Vector2 pivot  = rectTransform.pivot;
        float u = (localPoint.x / size.x) + pivot.x;
        float v = (localPoint.y / size.y) + pivot.y;

        // 3) minimapCamera Viewport 기준 Ray 생성
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0));
        
        // 4) 지면 평면과 교차
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        // xz 평면. Vector3.up과 수직인 0,0,0을 지나는 평면 만들기
        if (ground.Raycast(ray, out float enter)) // 여기 out enter는 "거리"
        {
            Vector3 hitPoint = ray.GetPoint(enter); // GetPoint는 거리로부터 ray부딪힌 지점을 반환하는 메서드
            cameraController.MoveCameraTo(hitPoint);
        }
    }
}