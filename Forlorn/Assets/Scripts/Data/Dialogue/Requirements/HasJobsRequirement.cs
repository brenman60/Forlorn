using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Jobs", fileName = "Has Jobs")]
public class HasJobsRequirement : DialogueRequirement
{
    [SerializeField] private List<JobRequirement> requirements = new List<JobRequirement>();

    public override bool MeetsRequirement()
    {
        bool isValid = true;

        JobManager jobManager = RunManager.Instance.jobManager;
        foreach (JobRequirement requirement in requirements)
        {
            bool hasJob = jobManager.holdingJobs.ContainsKey(requirement.job);

            switch (requirement.type)
            {
                case JobRequirementType.CantHave:
                    if (hasJob)
                        isValid = false;
                    break;
                case JobRequirementType.NeedsToHave:
                    if (!hasJob)
                        isValid = false;
                    break;
            }
        }

        return isValid;
    }

    [Serializable]
    protected struct JobRequirement
    {
        public Job job;
        public JobRequirementType type;
    }

    protected enum JobRequirementType
    {
        CantHave,
        NeedsToHave,
    }
}
