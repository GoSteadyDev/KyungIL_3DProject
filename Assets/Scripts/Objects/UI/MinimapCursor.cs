using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 마우스 포인터가 미니맵 위에 있을 때 커서 표시
[RequireComponent(typeof(RectTransform))]
public class MinimapCursor : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private Image cursorImage;      // 자식에 둔 Image
    private RectTransform rectTrans;                 // 미니맵 RawImage
    private RectTransform cursorRect;                // 커서 Image

    private void Awake()
    {
        rectTrans   = GetComponent<RectTransform>();
        cursorRect  = cursorImage.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorImage.gameObject.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // 1) Screen -> Local (RawImage 기준)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTrans,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPos) == false)
            return;

        // 2) 로컬 좌표 그대로 커서에 할당 (pivot 동일하게 설정하기)
        cursorRect.anchoredPosition = localPos;
    }
}