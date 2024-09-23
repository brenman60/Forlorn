using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/New Job", fileName = "New Job")]
public class Job : ScriptableObject
{
    public List<JobRank> ranks = new List<JobRank>()
    {
        new JobRank("Template Rank", 0, 0),
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

    [Header("Statistics")]
    public float paycheckAmount;

    public JobRank(string name, int rankLevel, float paycheckAmount)
    {
        this.name = name;
        this.rankLevel = rankLevel;
        this.paycheckAmount = paycheckAmount;
    }
}
