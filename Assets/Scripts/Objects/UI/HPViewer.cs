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

        GameObject viewerGO = GameObject.Instantiate(prefab, uiRoot);
        HPViewer viewer = viewerGO.GetComponent<HPViewer>();
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
    [SerializeField] private bool isStatic = false;
    
    private IDamageable hpTarget;   // hp 불러오기 위한 변수
    private Transform followTarget; // transform 불러기 위한 변수

    public void Setup(IDamageable target, Transform follow)
    {
        hpTarget = target;
        followTarget = follow;
    
        // 한 번만 계산해서 고정된 위치에 배치
        Vector3 screenPos = Camera.main.WorldToScreenPoint(follow.position);
        transform.position = screenPos;
    }
    private void Update()
    {
        if (hpTarget == null)
        {
            Destroy(gameObject);
            return;
        }
        
        float value = hpTarget.CurrentHP / hpTarget.MaxHP;
        hpSlider.value = Mathf.Clamp01(value);

        if (value <= 0.01f)
            hpSlider.value = 0f;
        
        // ✅ 색상 변경 로직
        if (value > 0.6f)
        {
            fillImage.color = Color.green;
        }
        else if (value > 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
        
        if (isStatic == false && followTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + hpViewerPosition);
            transform.position = screenPos;
        }
    }
    
    private void LateUpdate()
    {
        if (isStatic && followTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + hpViewerPosition);
            transform.position = screenPos;
        }
    }
}