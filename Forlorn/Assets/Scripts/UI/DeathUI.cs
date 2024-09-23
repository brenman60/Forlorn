using System;
using TMPro;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    public static DeathUI Instance { get; private set; }
    public static bool PlayerDead;

    [SerializeField] private CanvasGroup infoPanel;
    [SerializeField] private TextMeshProUGUI totalTimeText;

    private CanvasGroup canvasGroup;
    private float infoPanelLerp;

    private void Awake()
    {
        Instance = this;
        PlayerDead = false;
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, PlayerDead ? 1f : 0f, Time.deltaTime * 10f);
        canvasGroup.interactable = PlayerDead;
        canvasGroup.blocksRaycasts = PlayerDead;

        if (PlayerDead) infoPanelLerp += Time.deltaTime / 15;
        else infoPanelLerp = 0;
        infoPanel.alpha = Mathf.Lerp(0, 1, infoPanelLerp);
    }

    public void PlayerDeath()
    {
        PlayerDead = true;
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
