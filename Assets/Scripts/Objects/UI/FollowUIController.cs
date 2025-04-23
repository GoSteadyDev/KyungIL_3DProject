using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUIController : MonoBehaviour
{
    [SerializeField] private FollowUI damageTextPrefab;
    [SerializeField] private Canvas followCanvas;
    [SerializeField] private float UIduration = 1f;
    
    private void Start()
    {
        KillEventSystem.Instance.OnKill += HandleKillEvent;
        CombatSystem.Instance.OnCombatEvent += HandleCombatEvent;
    }

    private void OnDestroy()
    {
        // 구독 해제
        CombatSystem.Instance.OnCombatEvent -= HandleCombatEvent;
        KillEventSystem.Instance.OnKill -= HandleKillEvent;
    }

    private void HandleKillEvent(KillEvent killEvent)
    {
        FollowUI ui = Instantiate(damageTextPrefab, followCanvas.transform);
        ui.SetFloating(
            killEvent.Position,
            $"+{killEvent.GoldReward}",
            UIduration,
            Color.yellow
        );
    }
    
    private void HandleCombatEvent(CombatEvent combatEvent)
    {
        if (combatEvent.Receiver == null) return;

        FollowUI ui = Instantiate(damageTextPrefab, followCanvas.transform);
        ui.Set(
            combatEvent.Receiver.transform,
            Mathf.RoundToInt(combatEvent.Damage).ToString(),
            0.75f,
            Color.red
        );
    }
}
