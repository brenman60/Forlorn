using System;
using System.Collections.Generic;
using TypeReferences;
using UnityEngine;

[CreateAssetMenu(menuName = "City/New Job", fileName = "New Job")]
public class Job : ScriptableObject
{
    [Header("Customization")]
    public string visibleName;
    public Color visibleColor;

    [Header("Statistics")]
    [Range(0f, 1f), Tooltip("Higher values = harder application (0.9 = 90% failure, 0.2 = 20% failure)")] public float applicationDifficulty = 0.5f;

    public List<JobRank> ranks = new List<JobRank>()
    {
        new JobRank("TemplateRank", 0, "Template Rank", Color.white, 0, new List<StatType>(), new List<TypeReference>()),
    };
}

// Job Rank is basically just your position at the job (ex. Junior, Senior, etc (obviously will have specific roles in certain jobs))
// Job Ranks with the same rankLevel will be shown together as an option for players, some jobs will provide more benefitial effects and whatnot
// rankLevel will probably dictate the amount of points needed to reach that rank
[Serializable]
public struct JobRank
{
    [Header("Information")]
    public string name;
    public int rankLevel;
    public string visibleName;
    public Color visibleColor;

    [Header("Statistics")]
    public float payPerHour;
    public List<StatType> valuedStats;

    [Header("Effects")]
    [Inherits(typeof(StatModifier))] public List<TypeReference> modifiers;

    public JobRank(string name, int rankLevel, string visibleName, Color visibleColor, float paycheckAmount, List<StatType> valuedStats, List<TypeReference> modifiers)
    {
        this.name = name;
        this.rankLevel = rankLevel;
        this.visibleName = visibleName;
        this.visibleColor = visibleColor;
        this.payPerHour = paycheckAmount;
        this.valuedStats = valuedStats;
        this.modifiers = modifiers;
    }
}
