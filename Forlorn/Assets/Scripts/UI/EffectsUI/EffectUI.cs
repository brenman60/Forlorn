using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public Effect effect;

    [SerializeField] private EffectUIDatas effectUIDatas;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI timerText;

    private EffectUIData effectData;

    private void Start()
    {
        effectData = effectUIDatas.GetEffectDataByType(effect.GetType());
        iconImage.sprite = effectData.icon;
    }

    private void Update()
    {
        timerText.text = effect.timeLeft.ToString();
    }
}
