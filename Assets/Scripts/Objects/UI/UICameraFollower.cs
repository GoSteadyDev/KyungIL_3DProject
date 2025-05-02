using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraFollower : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            transform.position = targetCamera.transform.position;
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}
