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
    public bool changesMaxValue;
}

[Serializable]
public struct SkillStatCost
{
    public StatType statType;
    /*
       When not in max amount, it simply removes the amount if not percentage (-15 = -15) and in percentage it removes that given percent (15% of 100 = 15).
       While in max amount, it either removes the amount normally from the max, or removes the percentage, but in decimal form. Meaning to remove 15% of the max, it must be 1 - 0.15 (0.85),
       and to add 15%, it must be 1 + 0.15 (1.15). This is due to the math of multiplying the max by a number (since percentages wont really work).
    */
    public float requiredAmount;
    public bool isPercentage;
    public bool removesAmount;
    public bool removesFromMaxAmount;
    [TextArea(4, int.MaxValue)] public string displayText;
}

[Serializable]
public struct SkillItemCost
{
    public Item item;
    public int requiredAmount;
    public bool removesAmount;
    [TextArea(2, int.MaxValue)] public string displayText;
}
