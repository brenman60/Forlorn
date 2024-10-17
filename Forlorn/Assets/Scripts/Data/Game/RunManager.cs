using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public ApartmentManager apartmentManager { get; private set;  }

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
            Instance.apartmentManager = new ApartmentManager();

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(TickManagers), 1f, 1f);

        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
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
        ObjectivesList.Instance.ClearAll();

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

    public void SetRestStatus(bool status) => apartmentManager.SetRestStatus(status);

    public void PurchaseApartment(string name) => apartmentManager.PurchaseApartment(name);

    public void ApplyForDocuments(string itemName)
    {
        taskManager.StartTask(
            new TimeTask(
                "documentsApplication",
                "Offical Documents Application",
                60f * 2.5f,
                TaskType.DocumentsApplication,
                new Dictionary<string, object>()
                {
                    ["Item"] = itemName,
                }
            ));
    }

    public void ApplyForJob(string jobName)
    {
        Job selectedJob = jobs.GetJobByName(jobName);
        taskManager.StartTask(
            new TimeTask(
                selectedJob.name,
                selectedJob.visibleName + " Application", 
                60f * 0.5f,
                TaskType.JobApplication,
                new Dictionary<string, object>() 
                {
                    ["Job"] = jobName,
                }
            ));

        JobManager.InvokeApplicationsChanged();
    }

    public async void ClockIntoJob(Job job)
    {
        if (jobManager.daysShifts.Contains(job)) jobManager.daysShifts.Remove(job);

        float totalHours = (GameManager.Instance.gameTime / (GameManager.dayLength * 60)) * 24f;
        int currentHour = Mathf.FloorToInt(totalHours);
        int currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60);

        ShiftTime shiftClockIn = new ShiftTime(currentHour, currentMinute);
        ShiftTime startTime = jobManager.holdingJobs[job].startTime;
        int hoursLate = Mathf.Abs(shiftClockIn.hour - startTime.hour);
        int minutesLate = Mathf.Abs(shiftClockIn.minute - startTime.minute);
        int totalMinutesLate = (hoursLate * 60) + minutesLate;

        // because of this math its probably a good range to have players be fired at around -1000 points
        // since being late for 90 minutes (with 1x point mult) can put you down -1227 points (which shouldn't instantly kill players since they should've built up points before then)
        float pointMultiplier = statManager.stats[StatType.JobPointMultiplier].currentValue;
        EmploymentInformation employmentInformation = jobManager.holdingJobs[job];
        bool playerIsLate = totalMinutesLate > JobManager.lateThreshold;
        if (playerIsLate) // player is late for job (ruin points :))))
        {
            ObjectivesList.Instance.TryCompleteObjective(job.name + "shift", true);

            int minutesBeyondTheshold = totalMinutesLate - JobManager.lateThreshold;
            // math note: pointMultiplier is like insanely beneficial up to 2x but falls off almost instantly
            // so cool meta i guess
            employmentInformation.points += Mathf.RoundToInt((0.1f * -(minutesBeyondTheshold * minutesBeyondTheshold)) / (pointMultiplier / 1.5f));
        }
        else // player was on time (reward with points :((()
        {
            ObjectivesList.Instance.TryCompleteObjective(job.name + "shift", false);
            Player.Instance.gameObject.SetActive(false);
            Player.Instance.movementLocked = true;
            TimeScaleManager.AddInfluence("job" + job.name, 5f);

            int endTimeHour = employmentInformation.endTime.hour;
            int endTimeMinute = employmentInformation.endTime.minute;
            var (hour, minute, isPM) = GameManager.Instance.RealTimeToDayTime(GameManager.Instance.gameTime);
            if (isPM && hour != 12) hour += 12;

            while (hour < endTimeHour || minute < endTimeMinute)
            {
                (hour, minute, isPM) = GameManager.Instance.RealTimeToDayTime(GameManager.Instance.gameTime);
                if (isPM && hour != 12)
                    hour += 12;

                Player.Instance.gameObject.SetActive(false);
                await Task.Yield();
            }

            Player.Instance.gameObject.SetActive(true);
            Player.Instance.movementLocked = false;
            TimeScaleManager.RemoveInfluence("job" + job.name);
            employmentInformation.points += Mathf.RoundToInt(150 * pointMultiplier);
            statManager.stats[StatType.Money].currentValue += employmentInformation.rank.payPerHour * Mathf.Abs(employmentInformation.startTime.hour - employmentInformation.endTime.hour);
        }

        jobManager.holdingJobs[job] = employmentInformation;
    }

    public void EndJob(EmploymentInformation employmentInformation)
    {
        jobManager.holdingJobs.Remove(employmentInformation.job);
        JobManager.InvokeJobsChanged();
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[7]
        {
            statManager.GetSaveData(),
            jobManager.GetSaveData(),
            taskManager.GetSaveData(),
            apartmentManager.GetSaveData(),
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
        apartmentManager.PutSaveData(dataPoints[3]);
        if (!string.IsNullOrEmpty(dataPoints[4])) StartCoroutine(WaitForInventory(dataPoints[4]));
        if (!string.IsNullOrEmpty(dataPoints[5])) StartCoroutine(WaitForSkillsUI(dataPoints[5]));
        if (!string.IsNullOrEmpty(dataPoints[6])) StartCoroutine(WaitForObjectivesList(dataPoints[6]));
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
