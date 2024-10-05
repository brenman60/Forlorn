using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Stats", fileName = "Has Stats")]
public class HasStatsRequirement : DialogueRequirement
{
    [SerializeField] private List<StatRequirement> requirements = new List<StatRequirement>();

    public override bool MeetsRequirement()
    {
        bool hasStats = true;

        StatManager statManager = RunManager.Instance.statManager;
        foreach (StatRequirement requirement in requirements)
        {
            Stat stat = statManager.stats[requirement.stat];
            float currentPercentage = stat.maxValue * (requirement.amount / 100);
            if (requirement.isPercentage && currentPercentage < requirement.amount)
                hasStats = false;
            else if (stat.currentValue < requirement.amount)
                hasStats = false;
        }

        return hasStats;
    }

    public void RemoveRequirements()
    {
        StatManager statManager = RunManager.Instance.statManager;
        foreach (StatRequirement requirement in requirements)
        {
            if (!requirement.takesAmount) continue;

            Stat stat = statManager.stats[requirement.stat];
            if (requirement.isPercentage)
            {
                float percentage = stat.maxValue * (requirement.amount / 100);
                stat.currentValue -= percentage;
            }
            else
                stat.currentValue -= requirement.amount;
        }
    }

    [Serializable]
    protected struct StatRequirement
    {
        public StatType stat;
        public float amount;
        public bool isPercentage;
        public bool takesAmount;
    }
}
