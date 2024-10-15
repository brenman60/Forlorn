using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Upcoming Shift", fileName = "Has Upcoming Shift")]
public class HasUpcomingShiftRequirement : DialogueRequirement
{
    [SerializeField] private Job job;

    public override bool MeetsRequirement()
    {
        if (RunManager.Instance.jobManager.daysShifts.Contains(job))
        {
            EmploymentInformation employmentInformation = RunManager.Instance.jobManager.holdingJobs[job];
            var (currentHour, currentMinute, isPM) = GameManager.Instance.RealTimeToDayTime(GameManager.Instance.gameTime);
            if (isPM && currentHour != 12) currentHour += 12;

            float totalHourDiff = Mathf.Abs(currentHour - employmentInformation.startTime.hour);
            float totalMinuteDiff = Mathf.Abs(currentMinute - employmentInformation.startTime.minute);
            float totalMinutesEarly = (totalHourDiff * 60f) + totalMinuteDiff;

            if (totalMinutesEarly <= 60)
                return true;
            else
                return false;
        }
        else
            return false;
    }
}
