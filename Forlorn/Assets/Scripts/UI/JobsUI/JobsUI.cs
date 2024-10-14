using UnityEngine;

public class JobsUI : MonoBehaviour
{
    public static JobsUI Instance { get; private set; }

    [Header("Customization")]
    [SerializeField] private float openSpeed = 10f;
    [Header("References")]
    [SerializeField] private Transform jobsHolder;
    [SerializeField] private GameObject jobTemplate;

    private CanvasGroup canvasGroup;
    private bool open;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        JobManager.jobsChanged += JobsUpdated;
        JobsUpdated();
    }

    private void Update()
    {
        UpdateCanvasGroup();
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.unscaledDeltaTime * openSpeed);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }

    private void JobsUpdated()
    {
        foreach (Transform previousJob in jobsHolder) if (previousJob.gameObject != jobTemplate) Destroy(previousJob.gameObject);

        foreach (EmploymentInformation employmentInformation in RunManager.Instance.jobManager.holdingJobs.Values)
        {
            GameObject jobObject = Instantiate(jobTemplate, jobsHolder);
            JobUI jobUI = jobObject.GetComponent<JobUI>();
            jobUI.employmentInformation = employmentInformation;
            jobUI.UpdateInformation();
            jobObject.SetActive(true);
        }
    }

    public void Toggle()
    {
        open = !open;

        if (open)
            TimeScaleManager.AddPause(name);
        else
            TimeScaleManager.RemovePause(name);

        //if (open) SoundManager.Instance.PlayAudio("SkillsOpen", false, 0.5f);
    }

}
