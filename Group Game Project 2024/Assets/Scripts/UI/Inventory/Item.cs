using System;
using UnityEngine;
using TypeReferences;

[CreateAssetMenu(menuName = "Inventory/New Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string visibleName;
    [Space(15)]
    public UseStat[] useStats;
    public UseEffect[] useEffects;
    public UseModifier[] useModifiers;
}

[Serializable] 
public struct UseStat
{
    public StatType targetStat;
    public int change;
}

[Serializable]
public struct UseEffect
{
    [Inherits(typeof(Effect))]
    public TypeReference effect;
    public bool saveable;
}

[Serializable]
public struct UseModifier
{
    public StatType targetStat;
    public float modifierAmount;
    public bool isExponential;
}
