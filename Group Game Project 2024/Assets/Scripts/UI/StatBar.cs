using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [SerializeField] private StatType stat;
    [Space(10), SerializeField] private TextMeshProUGUI statPercentage;

    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        float maxStatValue = RunManager.Instance.statManager.stats[stat].maxValue;
        float statValue = RunManager.Instance.statManager.stats[stat].currentValue;
        slider.maxValue = maxStatValue;
        slider.value = statValue;

        statPercentage.text = Mathf.Clamp(Mathf.RoundToInt((statValue / maxStatValue) * 100), 0, 100) + "%";
    }
}
