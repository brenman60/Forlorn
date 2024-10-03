using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JobManager : ISaveData
{
    public List<EmploymentInformation> holdingJobs { get; private set; } = new List<EmploymentInformation>();

    private readonly Jobs jobs;

    public JobManager(Jobs jobs)
    {
        this.jobs = jobs;
    }

    public string GetSaveData()
    {
        List<string> jobSaves = new List<string>();
        foreach (EmploymentInformation employment in holdingJobs)
        {
            string[] employmentData = new string[6]
            {
                employment.job.name,
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
        }
    }
}
