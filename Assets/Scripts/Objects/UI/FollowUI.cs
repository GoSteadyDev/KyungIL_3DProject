using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class FollowUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.up * 5f;
    
    private RectTransform rectTransform;
    private TextMeshProUGUI text;
    
    private Transform target;
    private Vector3 startPos;
    private Vector3 endPos;

    private float duration = -1f;
    private float tempTime = 0f;
    private bool isFloating = false;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (duration < 0f) return;

        duration -= Time.deltaTime;

        if (duration <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (isFloating)
        {
            tempTime += Time.deltaTime;
            float t = tempTime / Mathf.Max(duration, 0.01f); // 방어
            rectTransform.position = Vector3.Lerp(startPos, endPos, t);
        }
        else if (target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
            text.enabled = screenPos.z > 0; // 카메라 벗어나면 UI 비활성화 처리
            rectTransform.position = screenPos;
        }
    }
    
    // 보통 Follow UI
    public void Set(Transform target, string content, float duration, Color color)
    {
        this.target = target;
        this.duration = duration;
        text.text = content;
        text.color = color;
        isFloating = false;
    }

    // 🟡 골드 텍스트 전용
    public void SetFloating(Vector3 worldPos, string content, float duration, Color color)
    {
        this.duration = duration;
        this.tempTime = 0f;
        this.target = null;
        this.isFloating = true;

        this.text.text = content;
        this.text.color = color;

        startPos = Camera.main.WorldToScreenPoint(worldPos);
        endPos = startPos + Vector3.up * 100f; // 조금 위로 떠오르기
        rectTransform.position = startPos;
    }
}
