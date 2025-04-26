using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public event Action OnGoldChanged;

    [SerializeField] private int gold = 500;
    public int Gold => gold;

    private void Awake()
    {
        Instance = this;
    }

    public void SetGold(int amount)
    {
        gold = amount;
        OnGoldChanged?.Invoke();
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        NotificationService.Notify($" {amount} gold added! You have {gold} gold.");
        OnGoldChanged?.Invoke();
    }

    public bool TrySpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            NotificationService.Notify($" -{amount} gold Spent. {gold} Left.");
            OnGoldChanged?.Invoke();
            return true;
        }
        else
        {
            NotificationService.Notify("You don't have enough gold!");
            return false;
        }
    }
}