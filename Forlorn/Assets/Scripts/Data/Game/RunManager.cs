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
        employmentInformation.startTime = GameManager.Instance.TimeToDayPercentage(9, 0, false);
        employmentInformation.endTime = GameManager.Instance.TimeToDayPercentage(5, 0, true);
        employmentInformation.workDays = new List<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
        };

        jobManager.holdingJobs.Add(employmentInformation);
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
        for (int i = 0; i < jobManager.holdingJobs.Count; i++)
        {
            EmploymentInformation information = jobManager.holdingJobs[i];
            if (information.job == job)
            {
                information.rank = nextRank;
                jobManager.holdingJobs[i] = information;
                break;
            }
        }
    }

    public void EndJob(EmploymentInformation employmentInformation)
    {
        jobManager.holdingJobs.Remove(employmentInformation);
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
    public float startTime;
    public float endTime;
    public int points;

    public EmploymentInformation(Job job, JobRank rank, List<DayOfWeek> workDays, float startTime, float endTime, int points)
    {
        this.job = job;
        this.rank = rank;
        this.workDays = workDays;
        this.startTime = startTime;
        this.endTime = endTime;
        this.points = points;
    }
}
