using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class HPViewerSpawner
{
    private static Transform uiRoot;

    public static void Initialize(Transform canvas)
    {
        uiRoot = canvas;
    }

    public static void CreateHPViewer(IDamageable target, Transform followTransform, GameObject prefab)
    {
        if (uiRoot == null)
        {
            return;
        }

        GameObject hpViewerPrefab = GameObject.Instantiate(prefab, uiRoot);
        HPViewer viewer = hpViewerPrefab.GetComponent<HPViewer>();
        viewer.Setup(target, followTransform);
    }
}

public class HPViewer : MonoBehaviour
{
    [Header("Viewer Settings")]
    [SerializeField] private Vector3 hpViewerPosition = Vector3.up * 15f;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage; // Fill Area → Fill에 연결 (Inspector에서)
    
    [Header("Building HP?")]
    [SerializeField] private bool isStatic = false; // 현재는 필요 없지만, 추후 풀링 적용할 때 필요
    
    private IDamageable hpTarget;   // hp 불러오기 위한 변수
    private Transform followTarget; // transform 불러기 위한 변수

    public void Setup(IDamageable target, Transform follow)
    {
        hpTarget = target;
        followTarget = follow;

        UpdatePosition(); // 최초 1회 위치 설정
    }

    private void Update()
    {
        if (hpTarget == null)
        {
            gameObject.SetActive(false);  // 위치 갱신 막기
            Destroy(gameObject);          // 다음 프레임에 파괴
            return;
        }

        UpdateHPBarVisual();

        if (followTarget != null)
        {
            UpdatePosition();
        }
    }

    private void UpdateHPBarVisual()
    {
        float value = Mathf.Clamp01(hpTarget.CurrentHP / hpTarget.MaxHP);
        hpSlider.value = value;

        if (value <= 0.01f) hpSlider.value = 0f;

        if (value > 0.6f) fillImage.color = Color.green;
        else if (value > 0.3f) fillImage.color = Color.yellow;
        else fillImage.color = Color.red;
    }

    private void UpdatePosition()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + hpViewerPosition);
        if (screenPos.z > 0f)
        {
            transform.position = screenPos;
        }
    }
}
