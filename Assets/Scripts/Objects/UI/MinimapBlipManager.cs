using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBlipManager : MonoBehaviour
{
    public static MinimapBlipManager Instance;
    
    [SerializeField] private Camera minimapCam;
    [SerializeField] private RectTransform blipParent;
    [SerializeField] private GameObject blipPrefab; // 하나만 있어도 OK!

    private readonly List<Transform> targets = new List<Transform>();
    private readonly List<RectTransform> blips   = new List<RectTransform>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTarget(Transform t, Color blipColor)
    {
        targets.Add(t);
        var imgRT = Instantiate(blipPrefab, blipParent)
            .GetComponent<RectTransform>();
        imgRT.GetComponent<UnityEngine.UI.Image>().color = blipColor;
        blips.Add(imgRT);
    }

    public void UnregisterTarget(Transform t)
    {
        int i = targets.IndexOf(t);
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
