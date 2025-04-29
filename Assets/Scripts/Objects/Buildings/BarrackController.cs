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

// 얘도 굳이 IHasInfoPanel 없으면 딱히 의미 없는 것 같은데 꼭 있어야할까?
// 적어도 마우스 눌렸을때, 배럭 버튼 패널도 함께 떠지게 처리하는 건 어떨지