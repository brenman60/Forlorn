using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunInfo : MonoBehaviour
{
    [HideInInspector] public string runId;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image dayStatusImage;
    [SerializeField] private Image timeStatusImage;
    [SerializeField] private TextMeshProUGUI lastPlayedText;
    [SerializeField] private TextMeshProUGUI createdText;

    private async void Start()
    {
#if !UNITY_WEBGL
        string runDataRaw = await SaveSystem.ReadFromFile(Path.Combine(SaveSystem.GetRunPath(runId), SaveSystem.runDataFile));
#elif UNITY_WEBGL
        string runDataRaw = PlayerPrefs.GetString(runId);
        if (string.IsNullOrEmpty(runDataRaw)) return;
#endif
        string[] runData = JsonConvert.DeserializeObject<string[]>(runDataRaw); // [2] = GameManager (day and time)
        string[] gameManagerData = JsonConvert.DeserializeObject<string[]>(runData[2]);
        dayText.text = "Day " + (int.Parse(gameManagerData[0]) + 1);
        SetTimeText(float.Parse(gameManagerData[1]));
        GetFileData();
    }

    private void SetTimeText(float gameTime) // Taken from TimeInfo.cs
    {
        float totalHours = (gameTime / (GameManager.dayLength * 60)) * 24f;
        float currentHour = Mathf.FloorToInt(totalHours);
        float currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60);

        string period = "AM";
        if (currentHour > 12)
        {
            period = "PM";
            currentHour -= 12;
        }

        timeText.text = currentHour + ":" + currentMinute.ToString("00") + " " + period;
    }

#if !UNITY_WEBGL
    private void GetFileData()
    {
        DirectoryInfo runDirectoryInfo = new DirectoryInfo(SaveSystem.GetRunPath(runId));
        lastPlayedText.text = "Last Played: " + runDirectoryInfo.LastWriteTime.ToShortDateString();
        createdText.text = "Created: " + runDirectoryInfo.CreationTime.ToShortDateString();
    }

    public async void ContinueRun()
    {
        GameManager.runId = runId;
        await SaveSystem.LoadRunData();
        TransitionUI.Instance.TransitionTo("Game");
    }
#elif UNITY_WEBGL
    private void GetFileData()
    {
        lastPlayedText.text = string.Empty;
        createdText.text = string.Empty;
    }

    public async void ContinueRun()
    {
        string[] runIds = runId.Split("\\", System.StringSplitOptions.RemoveEmptyEntries);
        GameManager.runId = runIds[runIds.Length - 2];
        await SaveSystem.LoadRunData();
        TransitionUI.Instance.TransitionTo("Game");
    }
#endif
}