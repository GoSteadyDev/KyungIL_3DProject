using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBlipManager : MonoBehaviour
{
    public static MinimapBlipManager Instance;  
    // 이거 굳이 싱글톤 쓰지 않고, KillEvent 구독으로 처리하는 게 좋을 것 같다?
    // 아.. 아니네 Register할 때는 또, 필요하겠구나.
    
    [SerializeField] private Camera minimapCam;
    [SerializeField] private RectTransform blipParent;
    [SerializeField] private GameObject blipPrefab; // 하나만 있어도 OK!

    private readonly List<RectTransform> blips = new List<RectTransform>();     // 자체 캔버스 (미니맵)에 쓸 RectTransform
    private readonly List<Transform> targets = new List<Transform>();           // 월드 좌표 (타겟)
    
    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTarget(Transform target, Color blipColor)
    {
        targets.Add(target);
        var imgRT = Instantiate(blipPrefab, blipParent).GetComponent<RectTransform>();
        imgRT.GetComponent<UnityEngine.UI.Image>().color = blipColor;
        blips.Add(imgRT);
    }

    public void UnregisterTarget(Transform target)
    {
        int i = targets.IndexOf(target);
        if (i < 0) return;
        Destroy(blips[i].gameObject);
        blips.RemoveAt(i);
        targets.RemoveAt(i);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 vp = minimapCam.WorldToViewportPoint(targets[i].position);
            float x = (vp.x - 0.5f) * blipParent.sizeDelta.x;
            float y = (vp.y - 0.5f) * blipParent.sizeDelta.y;
            blips[i].anchoredPosition = new Vector2(x, y);
        }
    }
}
