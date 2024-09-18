using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeBasedLight : MonoBehaviour
{
    [SerializeField] private float lightChangeSpeed = 0.25f;
    [SerializeField] private List<TimeLightSetting> lightSettings = new List<TimeLightSetting>();

    private TimeLightSetting selectedSetting = new TimeLightSetting(DayStatus.None, 0);

    private new Light2D light;

    private void Start()
    {
        GameManager.Instance.dayStatusChanged += TimeChanged;
    }

    private void Update()
    {
        if (selectedSetting.status != DayStatus.None)
        {
            light.intensity = Mathf.Lerp(light.intensity, selectedSetting.intensity, Time.deltaTime * lightChangeSpeed);
        }
    }

    private void TimeChanged(object sender, DayStatus newStatus)
    {
        foreach (TimeLightSetting setting in lightSettings)
            if (setting.status == newStatus)
            {
                selectedSetting = setting;
                break;
            }
    }

    [Serializable]
    private struct TimeLightSetting
    {
        public DayStatus status;
        public float intensity;

        public TimeLightSetting(DayStatus status, float intensity)
        {
            this.status = status;
            this.intensity = intensity;
        }
    }
}
