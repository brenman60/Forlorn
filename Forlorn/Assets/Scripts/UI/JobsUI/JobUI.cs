using Min_Max_Slider;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI jobNameText;
    [SerializeField] private TextMeshProUGUI rankNameText;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private TextMeshProUGUI endTimeText;
    [SerializeField] private MinMaxSlider shiftTimeSlider;
    [SerializeField] private List<Toggle> workingDays;

    [HideInInspector] public EmploymentInformation employmentInformation;

    public void UpdateInformation()
    {
        Job job = employmentInformation.job;
        JobRank rank = employmentInformation.rank;
        jobNameText.text = $"Job: <color={ColorUtility.ToHtmlStringRGB(job.visibleColor)}>{job.visibleName}</color>";
        rankNameText.text = $"Position: <color={ColorUtility.ToHtmlStringRGB(rank.visibleColor)}>{rank.visibleName}</color>";

        var (startHour, startMinute, startIsPM) = GameManager.Instance.PercentageToTime(employmentInformation.startTime);
        var (endHour, endMinute, endIsPM) = GameManager.Instance.PercentageToTime(employmentInformation.endTime);
        startTimeText.text = $"{startHour}:{startMinute} {(startIsPM ? "PM" : "AM")}";
        endTimeText.text = $"{endHour}:{endMinute} {(endIsPM ? "PM" : "AM")}";

        MinMaxSlider.MinMaxValues shiftTimeSliderValues = shiftTimeSlider.Values;
        shiftTimeSliderValues.minLimit = 0f;
        shiftTimeSliderValues.maxLimit = GameManager.dayLength * 60f;
        shiftTimeSliderValues.minValue = GameManager.Instance.TimeToSeconds(startHour, startMinute, startIsPM);
        shiftTimeSliderValues.maxValue = GameManager.Instance.TimeToSeconds(endHour, endMinute, endIsPM);
        shiftTimeSlider.SetValues(shiftTimeSliderValues, true);

        for (int i = 0; i < workingDays.Count; i++)
        {
            DayOfWeek dayOfWeek = (DayOfWeek)i;
            workingDays[i].isOn = employmentInformation.workDays.Contains(dayOfWeek);
        }
    }
}
