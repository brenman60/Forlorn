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

    public static bool paused;

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

    public const float dayLength = 8 + 1; // Should be input as the number of minutes, rather than seconds

    private DayStatus[] dayStatus_;

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
        isMobile = Application.isMobilePlatform;

        UpdateGameSpeed();
        UpdateGameTime();
        UpdateDayStatus();
    }

    private void UpdateGameSpeed()
    {
        bool instantPause = GameSettings.GetSetting<bool>(SettingType.InstantPause);
        Time.timeScale = instantPause ? (paused ? 0f : 1f) : Mathf.MoveTowards(Time.timeScale, paused ? 0f : 1f, Time.unscaledDeltaTime * 10f);
    }

    private void UpdateGameTime()
    {
        if (!gameActive || DeathUI.PlayerDead) return;

        if (gameTime >= (dayLength) * 60f) // Multiplying day length by 60 since day length is the number of minutes
        {
            gameTime = 0f;
            gameDays++;
            dayOfWeek = (DayOfWeek)(((int)dayOfWeek + 1) % 7);
        }
        else
            gameTime += Time.deltaTime; // This is very intentionally deltaTime and NOT unscaledDeltaTime (since there is pausing functionality)
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
    Morning = 15,
    Midday = 70,
    Night = 100,
}
