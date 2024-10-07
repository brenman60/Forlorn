using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JobManager : ISaveData
{
    public static event Action jobsChanged;
    public static event Action applicationsChanged;

    public Dictionary<Job, EmploymentInformation> holdingJobs { get; set; } = new Dictionary<Job, EmploymentInformation>();

    public readonly Jobs jobs;

    public JobManager(Jobs jobs)
    {
        this.jobs = jobs;
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

        return JsonConvert.SerializeObject(jobSaves);
    }

    public void PutSaveData(string data)
    {
        List<string> jobSaves = JsonConvert.DeserializeObject<List<string>>(data);
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
    }
}
