using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveData
{
    public static GameManager Instance { get; private set; }

    public static bool isMobile;

    // Probably going to set this once when the run is selected then never again
    public static string runId = "editor";

    public bool gameActive;

    public static readonly List<string> validGameScenes = new List<string>()
    {
        "Game",
    };

    public DayStatus dayStatus { get; private set; } = DayStatus.None;
    public event EventHandler<DayStatus> dayStatusChanged;

    public DayOfWeek dayOfWeek { get; private set; } = DayOfWeek.Monday;

    public float gameTime { get; private set; } = 0f;
    public int gameDays { get; private set; }

    private const float baseDayLength = 2f; // Should be input as the number of minutes, rather than seconds
    public static float dayLength
    {
        get
        {
            if (RunManager.Instance.statManager.stats.ContainsKey(StatType.DayLength))
                return baseDayLength * RunManager.Instance.statManager.stats[StatType.DayLength].currentValue;
            else
                return baseDayLength * RunManager.Instance.statManager.defaultStats[StatType.DayLength].currentValue;
        }
    }

    private DayStatus[] dayStatus_;

    [SerializeField] private bool forceMobile;

    private void Awake()
    {
        Instance = this;

        dayStatus_ = (DayStatus[])Enum.GetValues(typeof(DayStatus));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/GameManager"));
            managerObject.name = "GameManager";

            GameManager manager = managerObject.GetComponent<GameManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);

            SceneManager.sceneLoaded += SceneLoaded;
        }
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Instance.gameActive = validGameScenes.Contains(scene.name);
    }

    private void Update()
    {
        isMobile = Application.isMobilePlatform || forceMobile;

        if (gameActive)
        {
            UpdateGameTime();
            UpdateDayStatus();
        }
    }

    private void UpdateGameTime()
    {
        if (!gameActive || GameEndingUI.gameFinished) return;

        if (gameTime >= (dayLength) * 60f) // Multiplying day length by 60 since day length is the number of minutes
        {
            gameTime = 0f;
            gameDays++;
            dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);

            RunManager.Instance.jobManager.AddDayShifts();
        }
        else
        {
            gameTime += Time.deltaTime; // This is very intentionally deltaTime and NOT unscaledDeltaTime (since there is pausing functionality)
        }
    }

    public void ProgressGameTime(int hour, int minute)
    {
        gameTime += TimeToSeconds(hour, minute, false);
    }

    private void UpdateDayStatus()
    {
        float dayPercentage = (gameTime / (dayLength * 60f)) * 100f;
        foreach (DayStatus status in dayStatus_)
            if (dayPercentage < (int)status)
            {
                if (dayStatus != status && dayStatusChanged != null)
                    dayStatusChanged.Invoke(this, status);

                dayStatus = status;
                break;
            }
    }

    public float TimeToSeconds(int hour, int minute, bool isPM)
    {
        if (!isPM && hour == 12) hour = 0;
        if (isPM && hour != 12) hour += 12;

        return (hour * (DayMinuteToRealSecond() * 60)) + (minute * DayMinuteToRealSecond());
    }

    public float GameTimeToDayPercentage(int hour, int minute, bool isPM)
    {
        float timeInSeconds = TimeToSeconds(hour, minute, isPM);
        float totalDaySeconds = dayLength * 60;

        return timeInSeconds / totalDaySeconds * 100;
    }

    public (int hour, int minute, bool isPM) DayPercentageToGameTime(float percentage)
    {
        float totalDaySeconds = dayLength * 60;
        float timeInSeconds = (percentage / 100) * totalDaySeconds;

        return RealTimeToDayTime(timeInSeconds);
    }

    public (int hour, int minute, bool isPM) RealTimeToDayTime(float realTime)
    {
        float totalGameMinutes = (dayLength * 60) / (DayMinuteToRealSecond() / 60);

        float gameTimeInMinutes = realTime / DayMinuteToRealSecond();

        int gameHours = Mathf.FloorToInt(gameTimeInMinutes / 60);
        int gameMinutes = Mathf.FloorToInt(gameTimeInMinutes % 60);
        bool isPM = gameHours >= 12;

        gameHours = gameHours % 12;
        if (gameHours == 0) gameHours = 12;

        return (gameHours, gameMinutes, isPM);
    }

    public float DayTimeToRealTime(int hours, int minutes)
    {
        if (dayLength <= 0) throw new InvalidOperationException("Day length must be greater than zero.");

        float realMinute = DayMinuteToRealSecond();
        float realHour = realMinute * 60;

        float realHours = hours * realHour;
        float realMinutes = realMinute * minutes;

        return realHours + realMinutes;
    }

    public float DayMinuteToRealSecond()
    {
        if (dayLength <= 0) throw new InvalidOperationException("Day length must be greater than zero.");

        float dayLengthSeconds = dayLength * 60;
        float hourLength = dayLengthSeconds / 24;
        return hourLength / 60;
    }

    public string GetSaveData()
    {
        string[] dataPoints = new string[3]
        {
            gameDays.ToString(),
            gameTime.ToString(),
            ((int)dayOfWeek).ToString(),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);
        gameDays = int.Parse(dataPoints[0]);
        gameTime = float.Parse(dataPoints[1]);
        dayOfWeek = (DayOfWeek)int.Parse(dataPoints[2]);
    }

    private void OnApplicationQuit()
    {
        if (gameActive)
        {
            SaveSystem.SaveGlobal();
            SaveSystem.SaveRunData();
        }
    }
}

public enum DayStatus // Values assigned to the enums represent the amount of day that needs to pass before it isn't that status anymore
{
    None = 0,
    Morning = 30,
    Midday = 85,
    Night = 100,
}
