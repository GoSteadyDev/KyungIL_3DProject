using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent
{
    public GameObject Sender;
    public GameObject Receiver;
    public Collider Collider;
    public Vector3 HitPosition;
    public float Damage;
}