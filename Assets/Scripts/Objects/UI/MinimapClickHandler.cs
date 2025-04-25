using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("미니맵 전용 카메라(Orthographic)")]
    [SerializeField] private Camera minimapCamera;
    [Tooltip("메인 카메라 제어 스크립트")]
    [SerializeField] private CameraControlSystem cameraController;
    [Tooltip("지상 평면 Y 좌표")]
    [SerializeField] private float groundY = 0f;

    // RawImage의 RectTransform
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1) RawImage 로컬 좌표로 변환
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            ))
            return;

        // 2) 로컬 → 0~1 UV 좌표로 변환
        Vector2 size   = rectTransform.rect.size;
        Vector2 pivot  = rectTransform.pivot;
        float u = (localPoint.x / size.x) + pivot.x;
        float v = (localPoint.y / size.y) + pivot.y;

        // 3) minimapCamera Viewport 기준 Ray 생성
        Ray ray = minimapCamera.ViewportPointToRay(new Vector3(u, v, 0));

        // 4) 지면 평면과 교차
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            cameraController.MoveCameraTo(hitPoint);
        }
        // }
        //
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     Debug.Log("Point?");
        //     
        //     // 1) 미니맵 카메라로부터 Ray 생성
        //     Ray ray = minimapCamera.ScreenPointToRay(eventData.position);
        //
        //     // 2) y=groundY 평면 정의
        //     Plane ground = new Plane(Vector3.up, new Vector3(0, groundY, 0));
        //
        //     // 3) 평면과 교차점 계산
        //     if (ground.Raycast(ray, out float enter))
        //     {
        //         Vector3 hitPoint = ray.GetPoint(enter);
        //         Debug.Log($"Minimap hit world point: {hitPoint}");
        //         // 4) 메인 카메라 이동 호출
        //         cameraController.MoveCameraTo(hitPoint);
        //     }
        //     else
        //     {
        //         Debug.Log("Ground.Raycast failed!");
        //     }
    }
}