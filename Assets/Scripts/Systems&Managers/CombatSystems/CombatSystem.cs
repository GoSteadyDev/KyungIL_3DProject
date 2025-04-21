using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }
    
    // 외부에서 구독할 수 있는 이벤트
    public event Action<CombatEvent> OnCombatEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCombatEvent(CombatEvent combatEvent)
    {
        var damageable = combatEvent.Receiver.GetComponent<IDamageable>();
        
        if (damageable != null)
        {
            Debug.Log("Adding combat");
            damageable.TakeDamage(combatEvent.Damage);
        }

        // 이펙트, 사운드, 로그 처리 등도 여기에 추가
        OnCombatEvent?.Invoke(combatEvent);
    }
}
