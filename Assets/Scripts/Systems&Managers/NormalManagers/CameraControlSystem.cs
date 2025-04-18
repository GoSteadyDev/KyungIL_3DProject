using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class CameraControlSystem : MonoBehaviour
{
    [Header("Cam MoveSettings")]
    public float moveSpeed = 50f;
    public float borderThickness = 10f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minFOV = 30f;
    public float maxFOV = 45f;

    [Header("MoveRange Settings")]
    public float minX = -90f;
    public float maxX = 90f;
    public float minZ = -100f;
    public float maxZ = 135f;

    private Camera cam;
    private float fixedY; // y 위치 고정값

    private void Start()
    {
        cam = Camera.main;
        fixedY = transform.position.y;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 pos = transform.position;
        Vector3 dir = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        // 화면 가장자리 감지
        if (mousePos.x >= Screen.width - borderThickness) dir += Vector3.right;
        if (mousePos.x <= borderThickness) dir += Vector3.left;
        if (mousePos.y >= Screen.height - borderThickness) dir += Vector3.forward;
        if (mousePos.y <= borderThickness) dir += Vector3.back;

        // 카메라 이동 방향은 회전된 카메라 기준이 아닌, 월드 기준이므로 간단
        pos += dir.normalized * (moveSpeed * Time.deltaTime);

        // y 고정
        pos.y = fixedY;

        // 이동 제한
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2f, 0, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3((maxX - minX), 1, (maxZ - minZ));
        Gizmos.DrawWireCube(center, size);
    }
}
