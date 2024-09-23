using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunlightTime : MonoBehaviour
{
    private Light2D globalLight;
    private DayStatus dayStatus;

    private readonly Dictionary<DayStatus, float> statusIntensities = new Dictionary<DayStatus, float>()
    {
        [DayStatus.None] = 0,
        [DayStatus.Morning] = 0.02f,
        [DayStatus.Midday] = 0.25f,
        [DayStatus.Night] = 0.005f,
    };

    private void Start()
    {
        globalLight = GetComponent<Light2D>();
        GameManager.Instance.dayStatusChanged += DayStatusChanged;
        DayStatusChanged(null, GameManager.Instance.dayStatus);
    }

    private void DayStatusChanged(object sender, DayStatus newStatus)
    {
        dayStatus = newStatus;
    }

    private void Update()
    {
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, statusIntensities[dayStatus], Time.deltaTime * 0.2f);
    }
}
