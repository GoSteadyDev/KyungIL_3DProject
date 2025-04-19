using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEvent
{
    public GameObject Killer;     // 누가 죽였는가
    public GameObject Victim;     // 누가 죽었는가
    public Vector3 Position;      // 죽은 위치
    public int GoldReward;        // 보상 골드
}