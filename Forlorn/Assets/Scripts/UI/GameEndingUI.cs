using System;
using TMPro;
using UnityEngine;

public class GameEndingUI : MonoBehaviour
{
    public static GameEndingUI Instance { get; private set; }
    public static bool gameFinished;

    [SerializeField] private CanvasGroup infoPanel;
    [SerializeField] private TextMeshProUGUI finishingText;
    [SerializeField] private TextMeshProUGUI totalTimeText;

    private CanvasGroup canvasGroup;
    private float infoPanelLerp;

    private void Awake()
    {
        Instance = this;
        gameFinished = false;
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, gameFinished ? 1f : 0f, Time.deltaTime * 10f);
        canvasGroup.interactable = gameFinished;
        canvasGroup.blocksRaycasts = gameFinished;

        if (gameFinished) infoPanelLerp += Time.deltaTime / 15;
        else infoPanelLerp = 0;
        infoPanel.alpha = Mathf.Lerp(0, 1, infoPanelLerp);
    }

    public void FinishGame(string finishingMessage)
    {
        if (gameFinished) return;
        gameFinished = true;

        finishingText.text = finishingMessage;
        UpdateInfoPanel();
    }

    private void UpdateInfoPanel()
    {
        float totalDaysInMinutes = GameManager.Instance.gameDays * GameManager.dayLength;
        float dayLengthInMinutes = GameManager.Instance.gameTime / 60;
        float totalMinutes = totalDaysInMinutes + dayLengthInMinutes;
        float hours = totalMinutes / 60;

        if (hours < 1)
            totalTimeText.text = "Total Run Time: " + Math.Round(hours * 100) + " minutes";
        else
            totalTimeText.text = "Total Run Time: " + Math.Round(hours, 2) + " hours";
    }

    public void MainMenu()
    {
        TransitionUI.Instance.TransitionTo("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
