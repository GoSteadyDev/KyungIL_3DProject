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
    [SerializeField] private Slider hpSlider;
    // [SerializeField] private Vector3 screenOffset = new Vector3(0, 100f, 0);
    
    private IDamageable hpTarget;
    private Transform followTarget;

    public void Setup(IDamageable target, Transform follow)
    {
        hpTarget = target;
        followTarget = follow;
    }

    private void Update()
    {
        if (hpTarget == null || followTarget == null)
        {
            Destroy(gameObject);
            return;
        }
        float value = hpTarget.CurrentHP / hpTarget.MaxHP;
        hpSlider.value = Mathf.Clamp01(value);

        if (value <= 0.01f)
            hpSlider.value = 0f;
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position);
        transform.position = screenPos;
    }
}