using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunlightTime : MonoBehaviour
{
    private Light2D globalLight;

    private void Start()
    {
        globalLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        float newIntensity = 0f;
        switch (GameManager.Instance.dayStatus)
        {
            case DayStatus.Morning:
                newIntensity = 0.02f;
                break;
            case DayStatus.Midday:
                newIntensity = 0.25f;
                break;
            case DayStatus.Night:
                newIntensity = 0.005f;
                break;
        }

        globalLight.intensity = Mathf.Lerp(globalLight.intensity, newIntensity, Time.deltaTime * 0.2f);
    }
}
