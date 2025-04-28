using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeViewerController : MonoBehaviour
{
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private float viewerHeight = 0.1f;

    private Transform targetTransform;
    private float targetRange;
    private bool isFollowing = false;

    public void SetTarget(Transform target, float attackRange)
    {
        targetTransform = target;
        targetRange = attackRange;
        isFollowing = true;

        gameObject.SetActive(true);
        UpdateTransform();
    }

    public void Clear()
    {
        isFollowing = false;
        targetTransform = null;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!isFollowing || targetTransform == null) return;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Vector3 pos = targetTransform.position + Vector3.up * yOffset;
        transform.position = pos;

        float diameter = targetRange * 2f;
        transform.localScale = new Vector3(diameter, viewerHeight, diameter);
    }
}
