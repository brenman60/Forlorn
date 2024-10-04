using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : ISaveData
{
    public static event Action jobsChanged;

    private Dictionary<Job, EmploymentInformation> holdingJobs_ = new Dictionary<Job, EmploymentInformation>();
    public Dictionary<Job, EmploymentInformation> holdingJobs
    {
        get { return holdingJobs_; }
        set
        {
            if (holdingJobs_.Count != value.Count)
                jobsChanged?.Invoke();

            holdingJobs_ = value;
        }
    }

    private readonly Jobs jobs;

    public JobManager(Jobs jobs)
    {
        this.jobs = jobs;
    }

    public string GetSaveData()
    {
        List<string> jobSaves = new List<string>();
        foreach (KeyValuePair<Job, EmploymentInformation> jobInformation in holdingJobs_)
        {
            EmploymentInformation employment = jobInformation.Value;
            string[] employmentData = new string[6]
            {
                jobInformation.Key.name,
                employment.rank.name,
                employment.startTime.ToString(),
                employment.endTime.ToString(),
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

            information.startTime = float.Parse(employment[2]);
            information.endTime = float.Parse(employment[3]);
            information.workDays = JsonConvert.DeserializeObject<List<DayOfWeek>>(employment[4]);
            information.points = int.Parse(employment[5]);

            holdingJobs_.Add(information.job, information);
        }
    }
}
