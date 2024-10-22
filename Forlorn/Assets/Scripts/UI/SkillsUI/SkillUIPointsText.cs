using TMPro;
using UnityEngine;

public class SkillUIPointsText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    private void Start()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged += PointsChanged;
        PointsChanged(RunManager.Instance.statManager.stats[StatType.SkillPoints].currentValue);
    }

    private void PointsChanged(float newPoints)
    {
        pointsText.text = $"<color=#7fd9fa>{newPoints}</color> {(newPoints != 1 ? "Points" : "Point")}";
    }

    private void OnDestroy()
    {
        RunManager.Instance.statManager.stats[StatType.SkillPoints].valueChanged -= PointsChanged;
    }
}
