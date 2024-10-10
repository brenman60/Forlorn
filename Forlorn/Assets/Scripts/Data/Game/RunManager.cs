using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour, ISaveData
{
    public static RunManager Instance { get; private set; }
    public static bool isNewGame { get; set; }

    [SerializeField] private Jobs jobs;
    [SerializeField] private DialogueNodes dialogues;

    public StatManager statManager { get; private set; }
    public JobManager jobManager { get; private set; }
    public TaskManager taskManager { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/RunManager"));
            managerObject.name = "RunManager";

            RunManager manager = managerObject.GetComponent<RunManager>();
            Instance = manager;

            Instance.statManager = new StatManager();
            Instance.jobManager = new JobManager(Instance.jobs);
            Instance.taskManager = new TaskManager();

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(TickManagers), 1f, 1f);

        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        if (isNewGame)
        {
            isNewGame = false;
            StartCoroutine(FirstTimeLoad());
        }
    }

    private IEnumerator FirstTimeLoad()
    {
        yield return new WaitUntil(() => ObjectivesList.Instance != null);
        ObjectivesList.Instance.CreateNewObjective(new Objective("immigrationOfficeDiscover", "Find Immigration Office"));
        ObjectivesList.Instance.CreateNewObjective(new Objective("obtainJob", "Find Job"));
    }

    private void TickManagers()
    {
        if (GameManager.Instance.gameActive && TransitionUI.doneLoading)
        {
            statManager.Tick();
            taskManager.Tick();
            jobManager.Tick();
        }
    }

    public void ApplyForJob(string jobName)
    {
        Job selectedJob = jobs.GetJobByName(jobName);
        taskManager.StartTask(
            new TimeTask(
                selectedJob.visibleName + " Application", 
                60f * .15f,
                TaskType.JobApplication,
                new Dictionary<string, object>() 
                {
                    ["Job"] = jobName,
                }
            ));

        JobManager.InvokeApplicationsChanged();
    }

    public void RankUpJob(Job job, EmploymentInformation employmentInformation)
    {
        int indexOfCurrentRank = 0;
        for (int i = 0; i < job.ranks.Count; i++)
            if (job.ranks[i].name == employmentInformation.rank.name)
            {
                indexOfCurrentRank = i;
                break;
            }

        JobRank nextRank = job.ranks[indexOfCurrentRank];
        EmploymentInformation information = jobManager.holdingJobs[job];
        information.rank = nextRank;
        jobManager.holdingJobs[job] = information;
    }

    public void ClockIntoJob(Job job)
    {
        float totalHours = (GameManager.Instance.gameTime / (GameManager.dayLength * 60)) * 24f;
        int currentHour = Mathf.FloorToInt(totalHours);
        int currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60);

        ShiftTime shiftClockIn = new ShiftTime(currentHour, currentMinute);
        ShiftTime startTime = jobManager.holdingJobs[job].startTime;
        int hoursLate = Mathf.Abs(shiftClockIn.hour - startTime.hour);
        int minutesLate = Mathf.Abs(shiftClockIn.minute - startTime.minute);
        int totalMinutesLate = (hoursLate * 60) + minutesLate;

        // because of this math its probably a good range to have players be fired at around -1000 points
        // since being late for 60 minutes (with 1x point mult) can put you down -1350 points (which shouldn't instantly kill players since they should've built up points before then)
        float pointMultiplier = statManager.stats[StatType.JobPointMultiplier].maxValue;
        EmploymentInformation employmentInformation = jobManager.holdingJobs[job];
        bool playerIsLate = totalMinutesLate > JobManager.lateThreshold;
        if (playerIsLate) // player is late for job (ruin points :))))
        {
            int minutesBeyondTheshold = totalMinutesLate - JobManager.lateThreshold;
            // math note: pointMultiplier is like insanely beneficial up to 2x but falls off almost instantly
            // so cool meta i guess
            employmentInformation.points += Mathf.RoundToInt((0.25f * -(minutesBeyondTheshold * minutesBeyondTheshold)) / (pointMultiplier / 1.5f));
        }
        else // player was on time (reward with points :((()
            employmentInformation.points += Mathf.RoundToInt(150 * pointMultiplier);

        jobManager.holdingJobs[job] = employmentInformation;
        ObjectivesList.Instance.TryCompleteObjective(job.name + "shift", playerIsLate);
    }

    public void EndJob(EmploymentInformation employmentInformation)
    {
        jobManager.holdingJobs.Remove(employmentInformation.job);
        JobManager.InvokeJobsChanged();
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[6]
        {
            statManager.GetSaveData(),
            jobManager.GetSaveData(),
            taskManager.GetSaveData(),
            Inventory.Instance != null ? Inventory.Instance.GetSaveData() : string.Empty,
            SkillsUI.Instance != null ? SkillsUI.Instance.GetSaveData() : string.Empty,
            ObjectivesList.Instance != null ? ObjectivesList.Instance.GetSaveData() : string.Empty,
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        statManager.PutSaveData(dataPoints[0]);
        jobManager.PutSaveData(dataPoints[1]);
        taskManager.PutSaveData(dataPoints[2]);
        if (!string.IsNullOrEmpty(dataPoints[3])) StartCoroutine(WaitForInventory(dataPoints[3]));
        if (!string.IsNullOrEmpty(dataPoints[4])) StartCoroutine(WaitForSkillsUI(dataPoints[4]));
        if (!string.IsNullOrEmpty(dataPoints[5])) StartCoroutine(WaitForObjectivesList(dataPoints[5]));
    }

    private IEnumerator WaitForInventory(string inventoryData)
    {
        yield return new WaitUntil(() => Inventory.Instance != null);
        Inventory.Instance.PutSaveData(inventoryData);
    }

    private IEnumerator WaitForSkillsUI(string skillsData)
    {
        yield return new WaitUntil(() => SkillsUI.Instance != null);
        SkillsUI.Instance.PutSaveData(skillsData);
    }

    private IEnumerator WaitForObjectivesList(string objectivesData)
    {
        yield return new WaitUntil(() => ObjectivesList.Instance != null);
        ObjectivesList.Instance.PutSaveData(objectivesData);
    }
}

[Serializable]
public struct EmploymentInformation
{
    public Job job;
    public JobRank rank;
    public List<DayOfWeek> workDays;
    public ShiftTime startTime;
    public ShiftTime endTime;
    public int points;

    public EmploymentInformation(Job job, JobRank rank, List<DayOfWeek> workDays, ShiftTime startTime, ShiftTime endTime, int points)
    {
        this.job = job;
        this.rank = rank;
        this.workDays = workDays;
        this.startTime = startTime;
        this.endTime = endTime;
        this.points = points;
    }
}

[Serializable]
public struct ShiftTime
{
    public int hour;
    public int minute;

    public ShiftTime(int hour, int minute)
    {
        this.hour = hour;
        this.minute = minute;
    }
}
