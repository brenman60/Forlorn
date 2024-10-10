using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Upcoming Shift", fileName = "Has Upcoming Shift")]
public class HasUpcomingShiftRequirement : DialogueRequirement
{
    [SerializeField] private Job job;

    public override bool MeetsRequirement()
    {
        return RunManager.Instance.jobManager.daysShifts.Contains(job);
    }
}
