using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill", fileName = "New Skill")]
public class Skill : ScriptableObject
{
    public string saveName;
    public string visibleName;
    [TextArea(10, int.MaxValue)] public string displayDescription;
    [Header("Rewards")]
    public List<SkillModifier> modifiers = new List<SkillModifier>();
    [Header("Cost")]
    public List<SkillStatCost> skillStatCosts = new List<SkillStatCost>();
    public List<SkillItemCost> skillItemCosts = new List<SkillItemCost>();
}

[Serializable]
public struct SkillModifier
{
    public StatType statType;
    public float statChange;
    public bool isMultiplicative;
}

[Serializable]
public struct SkillStatCost
{
    public StatType statType;
    public float requiredAmount;
    public bool isPercentage;
    public bool removesAmount;
    [TextArea(2, int.MaxValue)] public string displayText;
}

[Serializable]
public struct SkillItemCost
{
    public Item item;
    public int requiredAmount;
    public bool removesAmount;
    [TextArea(2, int.MaxValue)] public string displayText;
}
