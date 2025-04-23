using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackController : MonoBehaviour, IHasInfoPanel
{
    [Header("패널 표시 정보")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string displayName = "Barracks";
    [TextArea]
    [SerializeField] private string description = "You can create Units";

    [Header("유닛 생성 시스템")]
    [SerializeField] private UnitSpawner unitSpawner;

    public string GetDisplayName() => displayName;
    public string GetDescription() => description;
    public Sprite GetIcon() => icon;
}