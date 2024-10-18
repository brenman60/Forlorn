using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUINextPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;

    private void Start()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPointsProgress].valueChanged += ProgressChanged;
    }

    private void ProgressChanged(float newValue)
    {
        int percentage = Mathf.RoundToInt((newValue / StatManager.skillPointProgressMax) * 100f);

        progressText.text = $"Next Point Progress <color=#7fd9fa>{percentage}%</color>";

        progressSlider.maxValue = StatManager.skillPointProgressMax;
        progressSlider.value = newValue;
    }

    private void OnDestroy()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPointsProgress].valueChanged -= ProgressChanged;
    }
}
