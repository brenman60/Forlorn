using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill", fileName = "New Skill")]
public class Skill : ScriptableObject
{
    public string saveName = "newSkill";
    public string visibleName = "New Skill";
    [TextArea(10, int.MaxValue)] public string displayDescription = "New Skill Description";
    [Header("Rewards")]
    public List<SkillModifier> modifiers = new List<SkillModifier>();
    [Header("Cost")]
    public int skillPointCost = 1;
}

[Serializable]
public struct SkillModifier
{
    public string modifierIdentifier;
    public StatType statType;
    public float statChange;
    public bool isMultiplicative;
    public bool changesMaxValue;
}
