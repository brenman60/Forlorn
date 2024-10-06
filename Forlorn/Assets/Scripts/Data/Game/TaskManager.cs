using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : ISaveData
{
    public static event Action<TimeTask, bool> tasksChanged;

    public List<TimeTask> tasks = new List<TimeTask>();

    public void StartTask(TimeTask task)
    {
        tasks.Add(task);
        tasksChanged?.Invoke(task, true);
    }

    public void Tick()
    {
        List<TimeTask> finishedTasks = new List<TimeTask>();
        for (int i = 0; i < tasks.Count; i++)
        {
            TimeTask task = tasks[i];
            task.currentTime -= 1f;

            if (task.currentTime <= 0)
            {
                finishedTasks.Add(task);
                switch (task.taskType)
                {
                    case TaskType.JobApplication:
                        JobApplicationCompleted(task.taskParameters);
                        break;
                    default:
                        Debug.LogError("TaskType '" + task.taskType.ToString() + "' not recognized.");
                        break;
                }
            }
        }

        foreach (TimeTask task in finishedTasks)
        {
            tasks.Remove(task);
            tasksChanged?.Invoke(task, false);
        }
    }

    private void JobApplicationCompleted(Dictionary<string, object> parameters)
    {
        Job job = RunManager.Instance.jobManager.jobs.GetJobByName(parameters["Job"].ToString());
        RunManager.Instance.jobManager.DetermineJobApp(job);
        JobManager.InvokeApplicationsChanged();
    }

    public string GetSaveData()
    {
        return JsonConvert.SerializeObject(tasks);
    }

    public void PutSaveData(string data)
    {
        tasks = JsonConvert.DeserializeObject<List<TimeTask>>(data);
    }
}

[Serializable]
public class TimeTask
{
    public string taskName;
    public float taskTime;
    public float currentTime;

    public TaskType taskType;
    public Dictionary<string, object> taskParameters;

    public TimeTask(string taskName, float taskTime, TaskType taskType, Dictionary<string, object> taskParameters)
    {
        this.taskName = taskName;
        this.taskTime = taskTime;
        this.currentTime = taskTime;
        this.taskType = taskType;
        this.taskParameters = taskParameters;
    }
}

public enum TaskType
{
    JobApplication,
}
