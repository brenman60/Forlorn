using UnityEngine;

public class JobsUIButton : MonoBehaviour
{
    [SerializeField] private ImportantIcon importantIcon;

    public void OnClick() => JobsUI.Instance.Toggle();

    private void Start()
    {
        Invoke(nameof(SetupChangedEvent), 1f);
    }

    private void SetupChangedEvent()
    {
        JobManager.jobsChanged += JobsChanged;
    }

    private void JobsChanged()
    {
        if (!JobsUI.Instance.open)
            importantIcon.SetImportance(true);
    }

    private void OnDestroy()
    {
        JobManager.jobsChanged -= JobsChanged;
    }
}
