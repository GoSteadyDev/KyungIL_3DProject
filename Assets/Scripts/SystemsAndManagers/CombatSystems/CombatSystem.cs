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
        // 여기서 컴포넌트를 직접 가져오는 선택을 했는데, CombatEvent에 IDamageable을 넣어줬더라면 더 좋은 설계가 되지 않았을까 싶기도?

        if (damageable != null)
        {
            damageable.TakeDamage(combatEvent.Damage); // -> CombatSystem이 직접 처리하는 CombatSystem의 메서드
        }

        // 이펙트, 사운드, 로그 처리 등도 여기에 추가
        OnCombatEvent?.Invoke(combatEvent); // -> CombatSystem을 구독한 FollowUIController의 메서드가 OnCombatEvent에 들어가게됨
    }
}