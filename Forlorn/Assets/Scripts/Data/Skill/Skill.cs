using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/New Skill", fileName = "New Skill")]
public class Skill : ScriptableObject
{
    public string saveName;
    public string visibleName;
    [TextArea(10, int.MaxValue)] public string displayDescription;
    [Space(15)]
    public List<SkillModifier> modifiers;
}

[Serializable]
public struct SkillModifier
{
    public StatType statType;
    public float statChange;
    public bool isMultiplicative;
}
