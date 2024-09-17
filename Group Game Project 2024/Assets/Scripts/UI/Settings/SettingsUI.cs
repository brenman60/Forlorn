using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance { get; private set; }

    [SerializeField] private GameObject[] tabContent;

    private CanvasGroup canvasGroup;
    private bool open;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitUI()
    {
        if (Instance == null)
        {
            GameObject uiObject = Instantiate(Resources.Load<GameObject>("UI/SettingsUI"));
            uiObject.name = "SettingsUI";

            SettingsUI ui = uiObject.GetComponent<SettingsUI>();
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
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, open ? 1f : 0f, Time.unscaledDeltaTime * 15f);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }

    public void ChangeTab(GameObject tab)
    {
        foreach (GameObject content in tabContent)
            content.SetActive(content == tab);
    }

    public void Toggle(bool toggle)
    {
        open = toggle;
    }

    public bool ToggleStatus()
    {
        return open;
    }
}
