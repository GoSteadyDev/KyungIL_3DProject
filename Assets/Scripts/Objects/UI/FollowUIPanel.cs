using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class FollowUIPanel : MonoBehaviour
{
    RectTransform rt;
    Camera cam;
    Vector2 screenCenter;

    Transform target;
    Vector3 worldOffset;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cam = Camera.main;
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    public void Initialize(Transform target, Vector3 worldOffset)
    {
        this.target = target;
        this.worldOffset = worldOffset;
        gameObject.SetActive(true);
        UpdatePosition(); // 최초 위치 갱신
    }

    void LateUpdate()
    {
        if (target != null)
            UpdatePosition();
    }

    void UpdatePosition()
    {
        // 정확히 타워 위치 + 오프셋을 화면 픽셀 좌표로 변환
        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = cam.WorldToViewportPoint(worldPos);
        screenPos.z = 0;

        // Viewport(0~1) 기준 위치를 실제 픽셀로 환산
        Vector2 anchored = new Vector2(
            screenPos.x * Screen.width - screenCenter.x,
            screenPos.y * Screen.height - screenCenter.y
        );

        rt.anchoredPosition = anchored;
    }

    public void Close()
    {
        target = null;
        gameObject.SetActive(false);
    }
}
