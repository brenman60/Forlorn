using TMPro;
using UnityEngine;

public class SkillUIPointsText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    private void Start()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged += PointsChanged;
    }

    private void PointsChanged(float newPoints)
    {
        pointsText.text = $"<color=#7fd9fa>{newPoints}</color> Points";
    }

    private void OnDestroy()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged -= PointsChanged;
    }
}
