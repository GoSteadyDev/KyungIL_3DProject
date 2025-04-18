using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUIController : MonoBehaviour
{
    [SerializeField] private FollowUI damageTextPrefab;
    [SerializeField] private Canvas followCanvas;

    private void Start()
    {
        CombatSystem.Instance.OnCombatEvent += HandleCombatEvent;
    }

    private void OnDestroy()
    {
        // 구독 해제
        CombatSystem.Instance.OnCombatEvent -= HandleCombatEvent;
    }

    private void HandleCombatEvent(CombatEvent combatEvent)
    {
        if (combatEvent.Receiver == null) return;

        // 데미지 텍스트 띄우기
        FollowUI ui = Instantiate(damageTextPrefab, followCanvas.transform);
        ui.Set(
            combatEvent.Receiver.transform,
            Mathf.RoundToInt(combatEvent.Damage).ToString(),
            1.5f,
            Color.red
        );
    }
}
