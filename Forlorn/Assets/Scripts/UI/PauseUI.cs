using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

    [SerializeField] private CanvasGroup exitRunButton;

    private CanvasGroup canvasGroup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject uiObject = Instantiate(Resources.Load<GameObject>("UI/PauseUI"));
            uiObject.name = "PauseUI";

            PauseUI ui = uiObject.GetComponent<PauseUI>();
            Instance = ui;

            DontDestroyOnLoad(uiObject);
        }
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Keybinds.GetKeybind(KeyType.Pause)) && !SettingsUI.Instance.ToggleStatus())
        {
            if (TimeScaleManager.HasPause(name))
                TimeScaleManager.RemovePause(name);
            else
                TimeScaleManager.AddPause(name);
        }

        UpdateCanvasGroup();

        exitRunButton.interactable = GameManager.Instance.gameActive;
    }

    private void UpdateCanvasGroup()
    {
        bool hasPause = TimeScaleManager.HasPause(name);
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, hasPause ? 1f : 0f, Time.unscaledDeltaTime * 25f);
        canvasGroup.interactable = hasPause;
        canvasGroup.blocksRaycasts = hasPause;
    }

    public void Resume()
    {
        TimeScaleManager.RemovePause(name);
    }

    public void OpenSettings()
    {
        SettingsUI.Instance.Toggle(true);
    }

    public void ExitRun()
    {
        SaveSystem.SaveRunData();

        TransitionUI.Instance.TransitionTo("MainMenu");
        TimeScaleManager.RemovePause(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
