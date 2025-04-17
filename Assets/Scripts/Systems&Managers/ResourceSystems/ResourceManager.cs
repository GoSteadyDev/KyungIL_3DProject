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

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke();
    }

    public bool TrySpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke();
            return true;
        }
        return false;
    }
}
