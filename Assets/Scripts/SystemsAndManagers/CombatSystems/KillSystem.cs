using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEventSystem : MonoBehaviour
{
    public static KillEventSystem Instance;
    public event Action<KillEvent> OnKill;

    private void Awake()
    {
        Instance = this;
    }

    public void Broadcast(KillEvent killEvent)
    {
        OnKill?.Invoke(killEvent);
    }
}