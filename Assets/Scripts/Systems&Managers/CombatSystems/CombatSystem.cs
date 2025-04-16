using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddCombatEvent(CombatEvent combatEvent)
    {
        var damageable = combatEvent.Receiver.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            damageable.TakeDamage(combatEvent.Damage);
        }

        // 이펙트, 사운드, 로그 처리 등도 여기에 추가
    }
}
