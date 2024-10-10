using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager : ISaveData
{
    public static int lateThreshold = 80; // the amount of time to pass from the start time for the player to be considered late

    public static event Action jobsChanged;
    public static event Action applicationsChanged;

    public Dictionary<Job, EmploymentInformation> holdingJobs { get; set; } = new Dictionary<Job, EmploymentInformation>();
    public List<Job> daysShifts { get; set; } = new List<Job>();

    public readonly Jobs jobs;

    public JobManager(Jobs jobs)
    {
        this.jobs = jobs;
    }

    public void Tick()
    {
        // Check for late jobs
        var (currentHour, currentMinute, isPM) = GameManager.Instance.RealTimeToDayTime(GameManager.Instance.gameTime);
        foreach (KeyValuePair<Job, EmploymentInformation> holdingJob in holdingJobs.ToArray())
        {
            if (!daysShifts.Contains(holdingJob.Key)) continue;

            int currentHour_ = currentHour;
            if (isPM && currentHour_ != 12) currentHour_ += 12;
            else if (!isPM && currentHour_ == 12) currentHour_ = 0;

            int hoursLate = Mathf.Clamp(currentHour_ - holdingJob.Value.startTime.hour, 0, int.MaxValue);
            int minutesLate = Mathf.Clamp(currentMinute - holdingJob.Value.startTime.minute, 0, int.MaxValue);
            int totalMinutesLate = (hoursLate * 60) + minutesLate;

            if (totalMinutesLate > lateThreshold)
            {
                RunManager.Instance.ClockIntoJob(holdingJob.Key);

                if (daysShifts.Contains(holdingJob.Key))
                    daysShifts.Remove(holdingJob.Key);
            }
        }
    }

    public void DetermineJobApp(Job job)
    {
        StatManager statManager = RunManager.Instance.statManager;
        float successChance = UnityEngine.Random.Range(0f, 1f);
        foreach (StatType weightedStat in job.ranks[0].valuedStats)
            successChance *= statManager.stats[weightedStat].maxValue;

        bool successful = successChance >= job.applicationDifficulty;
        if (successful)
            StartNewJob(job);
    }

    private void StartNewJob(Job job)
    {
        EmploymentInformation employmentInformation = new EmploymentInformation();
        employmentInformation.job = job;
        employmentInformation.rank = job.ranks[0];
        employmentInformation.startTime = new ShiftTime(9, 0);
        employmentInformation.endTime = new ShiftTime(5 + 12, 0);
        employmentInformation.workDays = new List<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
        };

        holdingJobs.Add(job, employmentInformation);
        InvokeJobsChanged();

        ObjectivesList.Instance.TryCompleteObjective("obtainJob");
    }

    public void AddDayShifts()
    {
        daysShifts.Clear();
        foreach (Job job in holdingJobs.Keys)
        {
            EmploymentInformation employmentInformation = holdingJobs[job];
            bool jobIsInPM = employmentInformation.startTime.hour > 12;
            ObjectivesList.Instance.CreateNewObjective(new Objective(job.name + "shift", $"Start {job.visibleName} Shift: {(jobIsInPM ? employmentInformation.startTime.hour - 12 : employmentInformation.startTime.hour)}:{employmentInformation.startTime.minute.ToString("00")} {(jobIsInPM ? "PM" : "AM")}"));
            daysShifts.Add(job);
        }
    }

    public void CallOut(Job job)
    {
        ObjectivesList.Instance.TryCompleteObjective(job.name + "shift");
        if (daysShifts.Contains(job))
            daysShifts.Remove(job);
    }

    public static void InvokeJobsChanged() => jobsChanged?.Invoke();

    public static void InvokeApplicationsChanged() => applicationsChanged?.Invoke();

    public string GetSaveData()
    {
        List<string> jobSaves = new List<string>();
        foreach (KeyValuePair<Job, EmploymentInformation> jobInformation in holdingJobs)
        {
            EmploymentInformation employment = jobInformation.Value;
            string[] employmentData = new string[6]
            {
                jobInformation.Key.name,
                employment.rank.name,
                JsonConvert.SerializeObject(employment.startTime),
                JsonConvert.SerializeObject(employment.endTime),
                JsonConvert.SerializeObject(employment.workDays),
                employment.points.ToString(),
            };

            jobSaves.Add(JsonConvert.SerializeObject(employmentData));
        }

        List<string> dayShiftsSaves = new List<string>();
        foreach (Job shift in daysShifts)
            dayShiftsSaves.Add(shift.name);

        string[] dataPoints = new string[2]
        {
            JsonConvert.SerializeObject(jobSaves),
            JsonConvert.SerializeObject(dayShiftsSaves),
        };

        return JsonConvert.SerializeObject(jobSaves);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);

        List<string> jobSaves = JsonConvert.DeserializeObject<List<string>>(dataPoints[0]);
        foreach (string employmentData in jobSaves)
        {
            string[] employment = JsonConvert.DeserializeObject<string[]>(employmentData);
            EmploymentInformation information = new EmploymentInformation();
            information.job = jobs.GetJobByName(employment[0]);

            foreach (JobRank rank in information.job.ranks)
                if (rank.name == employment[1])
                {
                    information.rank = rank;
                    break;
                }

            information.startTime = JsonConvert.DeserializeObject<ShiftTime>(employment[2]);
            information.endTime = JsonConvert.DeserializeObject<ShiftTime>(employment[3]);
            information.workDays = JsonConvert.DeserializeObject<List<DayOfWeek>>(employment[4]);
            information.points = int.Parse(employment[5]);

            holdingJobs.Add(information.job, information);
        }

        List<string> dayShiftsSaves = JsonConvert.DeserializeObject<List<string>>(dataPoints[1]);
        foreach (string dayShiftSave in dayShiftsSaves)
            daysShifts.Add(jobs.GetJobByName(dayShiftSave));
    }
}
