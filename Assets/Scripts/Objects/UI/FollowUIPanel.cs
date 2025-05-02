using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class FollowUIPanel : MonoBehaviour
{
    [SerializeField] private Camera uiCamera; // UICamera를 Inspector에서 연결
    [SerializeField] private Canvas parentCanvas;
    
    RectTransform rt;
    Camera cam;
    Vector2 screenCenter;

    Transform target;
    Vector3 worldOffset;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
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
            canvasRect,
            screenPos,
            uiCamera,
            out localPoint
        );

        if (success)
        {
            rt.anchoredPosition = localPoint;
        }
    }
    public void Close()
    {
        target = null;
        gameObject.SetActive(false);
    }
}
