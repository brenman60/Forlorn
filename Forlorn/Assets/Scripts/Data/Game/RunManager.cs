using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour, ISaveData
{
    public static RunManager Instance { get; private set; }

    [SerializeField] private Jobs jobs;
    [SerializeField] private DialogueNodes dialogues;

    public StatManager statManager;
    public JobManager jobManager;

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

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(TickStatManager), 1f, 1f);
    }

    private void TickStatManager()
    {
        if (GameManager.Instance.gameActive && TransitionUI.doneLoading) statManager.TickEffects();
    }

    public void ApplyForJob(string jobName, string successDialogue, string failureDialogue)
    {
        GameManager.Instance.ProgressGameTime(1, 30);
        Job selectedJob = jobs.GetJobByName(jobName);
        float successChance = UnityEngine.Random.Range(0f, 1f);
        foreach (StatType weightedStat in selectedJob.ranks[0].valuedStats)
            successChance *= statManager.stats[weightedStat].maxValue;

        bool successful = successChance >= selectedJob.applicationDifficulty;
        if (successful)
        {
            DialogueNode successNode = dialogues.GetDialogueNodeByName(successDialogue);
            DialogueUI.Instance.AddDialogue(successNode);
            StartNewJob(selectedJob);
        }
        else
        {
            DialogueNode failureNode = dialogues.GetDialogueNodeByName(failureDialogue);
            DialogueUI.Instance.AddDialogue(failureNode);
        }
    }

    public void StartNewJob(Job job)
    {
        EmploymentInformation employmentInformation = new EmploymentInformation();
        employmentInformation.job = job;
        employmentInformation.rank = job.ranks[0];
        employmentInformation.startTime = new ShiftTime(9, 0, false);
        employmentInformation.endTime = new ShiftTime(5, 0, true);
        employmentInformation.workDays = new List<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
        };

        jobManager.holdingJobs.Add(job, employmentInformation);
        JobManager.InvokeJobsChanged();
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

    public void EndJob(EmploymentInformation employmentInformation)
    {
        jobManager.holdingJobs.Remove(employmentInformation.job);
        JobManager.InvokeJobsChanged();
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[4]
        {
            statManager.GetSaveData(),
            jobManager.GetSaveData(),
            Inventory.Instance != null ? Inventory.Instance.GetSaveData() : string.Empty,
            SkillsUI.Instance != null ? SkillsUI.Instance.GetSaveData() : string.Empty,
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        statManager.PutSaveData(dataPoints[0]);
        jobManager.PutSaveData(dataPoints[1]);
        if (!string.IsNullOrEmpty(dataPoints[2])) StartCoroutine(WaitForInventory(dataPoints[2]));
        if (!string.IsNullOrEmpty(dataPoints[3])) StartCoroutine(WaitForSkillsUI(dataPoints[3]));
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
    public bool isPM;

    public ShiftTime(int hour, int minute, bool isPM)
    {
        this.hour = hour;
        this.minute = minute;
        this.isPM = isPM;
    }
}
