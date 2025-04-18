using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    private Transform target;
    private float duration = -1f;  // 기본값 음수로 설정
    private TextMeshProUGUI text;
    private RectTransform rectTransform;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Set(Transform target, string content, float duration, Color color)
    {
        this.target = target;
        this.duration = duration;
        text.text = content;
        text.color = color;
    }

    private void Update()
    {
        if (target == null || duration < 0f) return;

        duration -= Time.deltaTime;
        if (duration <= 0f)
        {
            Destroy(gameObject); // 추후 Pool로 교체
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 2f);
        text.enabled = screenPos.z > 0; // 📌 화면 밖이면 숨김
        rectTransform.position = screenPos;
    }
}

