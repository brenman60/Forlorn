using TMPro;
using UnityEngine;

public class TimeSpeedInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float currentTime = TimeScaleManager.currentTime;
        speedText.text = currentTime + "x";

        bool visible = currentTime > 1f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, visible ? 1f : 0f, Time.unscaledDeltaTime * 2.5f);
    }
}
