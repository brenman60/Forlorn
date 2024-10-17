using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager : ISaveData
{
    public const int lateThreshold = 80; // the amount of time to pass from the start time for the player to be considered late
    public const int fireThreshold = -1000; // job with points equal or below this will have player be fired

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
            if (!daysShifts.Contains(holdingJob.Key) || TimeScaleManager.HasInfluence("job" + holdingJob.Key.name)) continue;

            int currentHour_ = currentHour;
            if (isPM && currentHour_ != 12) currentHour_ += 12;
            else if (!isPM && currentHour_ == 12) currentHour_ = 0;

            int hoursLate = Mathf.Clamp(currentHour_ - holdingJob.Value.startTime.hour, 0, int.MaxValue);
            int minutesLate = Mathf.Clamp(currentMinute - holdingJob.Value.startTime.minute, 0, int.MaxValue);
            int totalMinutesLate = (hoursLate * 60) + minutesLate;

            if (totalMinutesLate > lateThreshold)
                RunManager.Instance.ClockIntoJob(holdingJob.Key);
        }

        // Check job points and rank up or fire
        foreach (KeyValuePair<Job, EmploymentInformation> holdingJob in holdingJobs.ToArray())
        {
            EmploymentInformation information = holdingJob.Value;
            JobRank nextRank = holdingJob.Key.ranks[Mathf.Clamp(holdingJob.Key.ranks.IndexOf(information.rank) + 1, 0, holdingJob.Key.ranks.Count - 1)];
            if (information.points <= fireThreshold)
            {
                holdingJobs.Remove(holdingJob.Key);
                jobsChanged?.Invoke();

                // Give notification of firing
                NotificationsUI.Instance.CreateNotification($"Fired from {holdingJob.Key.visibleName}...");
            }
            else if (information.points >= nextRank.rankLevel && nextRank.name != information.rank.name)
            {
                information.rank = nextRank;
                // gives your rank's level times 175 points. so two promotions will give 300 initial points on the third rank
                // should go rank 1 = 175, rank 2 = 350, rank 3 = 525, etc
                // should help soften blow from missing shifts
                information.points = Mathf.RoundToInt((holdingJob.Key.ranks.IndexOf(information.rank) + 1) * 175f); 
                holdingJobs[holdingJob.Key] = information;
                jobsChanged?.Invoke();

                // Give notification of rank up
                NotificationsUI.Instance.CreateNotification($"Promoted to {information.rank.visibleName}!");
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
        {
            StartNewJob(job);
            NotificationsUI.Instance.CreateNotification($"{job.visibleName} Application Successfull!");
        }
        else
            NotificationsUI.Instance.CreateNotification($"{job.visibleName} Application Denied...");
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
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
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
            if (employmentInformation.workDays.Contains(GameManager.Instance.dayOfWeek))
            {
                bool jobIsInPM = employmentInformation.startTime.hour > 12;
                ObjectivesList.Instance.CreateNewObjective(new Objective(job.name + "shift", $"Start {job.visibleName} Shift: {(jobIsInPM ? employmentInformation.startTime.hour - 12 : employmentInformation.startTime.hour)}:{employmentInformation.startTime.minute.ToString("00")} {(jobIsInPM ? "PM" : "AM")}"));
                daysShifts.Add(job);
            }
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

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);

        holdingJobs.Clear();
        daysShifts.Clear();

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
