using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

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
            GameManager.paused = !GameManager.paused;

        UpdateCanvasGroup();
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, GameManager.paused ? 1f : 0f, Time.unscaledDeltaTime * 25f);
        canvasGroup.interactable = GameManager.paused;
        canvasGroup.blocksRaycasts = GameManager.paused;
    }

    public void Resume()
    {
        GameManager.paused = false;
    }

    public void OpenSettings()
    {
        SettingsUI.Instance.Toggle(true);
    }

    public void ExitRun()
    {
        WorldGeneration.Instance.SaveSection();
        SaveSystem.SaveRunData();

        TransitionUI.Instance.TransitionTo("MainMenu");
        GameManager.paused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
