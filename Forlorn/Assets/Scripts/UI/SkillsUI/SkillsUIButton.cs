using UnityEngine;

public class SkillsUIButton : MonoBehaviour
{
    [SerializeField] private ImportantIcon importantIcon;

    public void OnClick() => SkillsUI.Instance.Toggle();

    private void Start()
    {
        Invoke(nameof(SetupChangedEvent), 1f);
    }

    private void SetupChangedEvent()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged += SkillPointsChanged;
    }

    private void SkillPointsChanged(float obj)
    {
        if (!SkillsUI.Instance.open)
            importantIcon.SetImportance(true);
    }

    private void OnDestroy()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged -= SkillPointsChanged;
    }
}
