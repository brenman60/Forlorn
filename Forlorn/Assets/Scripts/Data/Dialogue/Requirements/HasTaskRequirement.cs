using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Tasks", fileName = "Has Tasks")]
public class HasTaskRequirement : DialogueRequirement
{
    [SerializeField] private List<TaskRequirement> requirements = new List<TaskRequirement>();

    public override bool MeetsRequirement()
    {
        bool hasRequirements = true;

        TaskManager taskManager = RunManager.Instance.taskManager;
        foreach (TaskRequirement requirement in requirements)
        {
            bool taskValid = requirement.cantHave;
            foreach (TimeTask task in taskManager.tasks)
            {
                if (task.name == requirement.taskName && requirement.cantHave)
                {
                    taskValid = false;
                    break;
                }
                else if (task.name == requirement.taskName && !requirement.cantHave)
                {
                    taskValid = true;
                    break;
                }
            }

            if (!taskValid)
            {
                hasRequirements = false;
                break;
            }
        }

        return hasRequirements;
    }

    [Serializable] 
    private struct TaskRequirement 
    {
        public string taskName;
        public bool cantHave;
    }
}
