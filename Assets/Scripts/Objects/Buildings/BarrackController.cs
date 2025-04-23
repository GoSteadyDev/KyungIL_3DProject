using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackController : MonoBehaviour, IHasInfoPanel
{
    [Header("Panel Info")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string displayName = "Barracks";
    [TextArea]
    [SerializeField] private string description = "You can create Units";

    [Header("Unit SpawnSystem")]
    [SerializeField] private UnitSpawner unitSpawner;

    public string GetDisplayName() => displayName;
    public string GetDescription() => description;
    public Sprite GetIcon() => icon;
}