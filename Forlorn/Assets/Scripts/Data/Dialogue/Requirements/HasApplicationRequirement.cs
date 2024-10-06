using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Applications", fileName = "Has Applications")]
public class HasApplicationRequirement : DialogueRequirement
{
    [SerializeField] private List<Job> requirements = new List<Job>();

    public override bool MeetsRequirement()
    {
        bool noApplication = true;

        TaskManager taskManager = RunManager.Instance.taskManager;
        foreach (Job requirement in requirements)
            foreach (TimeTask task in taskManager.tasks)
                if (task.taskType == TaskType.JobApplication && task.taskParameters.TryGetValue("Job", out object jobName) && jobName.ToString() == requirement.name)
                {
                    noApplication = false;
                    break;
                }

        return noApplication;
    }
}
