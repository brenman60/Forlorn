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
    private JobManager jobManager;

    private bool loadingInfo;

    public void UpdateInformation()
    {
        loadingInfo = true;

        Job job = employmentInformation.job;
        JobRank rank = employmentInformation.rank;
        jobNameText.text = $"Job: <color=#{ColorUtility.ToHtmlStringRGB(job.visibleColor)}>{job.visibleName}</color>";
        rankNameText.text = $"Position: <color=#{ColorUtility.ToHtmlStringRGB(rank.visibleColor)}>{rank.visibleName}</color>";

        ReloadShiftTime();
        ReloadWorkingDays();

        jobManager = RunManager.Instance.jobManager;

        loadingInfo = false;
    }

    public void UpdateWorkingDays(Toggle day)
    {
        if (loadingInfo) return;
        DayOfWeek dayOfWeek = (DayOfWeek)int.Parse(day.name);

        if (day.isOn)
            employmentInformation.workDays.Add(dayOfWeek);
        else
            employmentInformation.workDays.Remove(dayOfWeek);

        UpdateJobInfo();
        ReloadWorkingDays();
    }

    public void UpdateStartTime(int minutes)
    {
        if (loadingInfo) return;

        ShiftTime startShiftTime = employmentInformation.startTime;
        startShiftTime.minute += minutes;
        if (startShiftTime.minute > 59)
        {
            startShiftTime.minute -= 60;
            startShiftTime.hour++;
        }
        else if (startShiftTime.minute < 0)
        {
            startShiftTime.minute += 60;
            startShiftTime.hour--;
        }

        if (startShiftTime.hour > 12)
        {
            startShiftTime.isPM = !startShiftTime.isPM;
            startShiftTime.hour -= 12;
        }
        else if (startShiftTime.hour < 1)
        {
            startShiftTime.isPM = !startShiftTime.isPM;
            startShiftTime.hour += 12;
        }

        if (!startShiftTime.isPM)
        {
            if (startShiftTime.hour < 6)
                startShiftTime.minute = 0;

            startShiftTime.hour = Mathf.Clamp(startShiftTime.hour, 6, 12);
        }

        employmentInformation.startTime = startShiftTime;

        UpdateJobInfo();
        ReloadShiftTime();
    }

    public void UpdateEndTime(int minutes)
    {
        if (loadingInfo) return;

        ShiftTime endShiftTime = employmentInformation.endTime;
        endShiftTime.minute += minutes;
        if (endShiftTime.minute > 59)
        {
            endShiftTime.minute -= 60;
            endShiftTime.hour++;
        }
        else if (endShiftTime.minute < 0)
        {
            endShiftTime.minute += 60;
            endShiftTime.hour--;
        }

        if (endShiftTime.hour > 12)
        {
            endShiftTime.isPM = !endShiftTime.isPM;
            endShiftTime.hour -= 12;
        }
        else if (endShiftTime.hour < 1)
        {
            endShiftTime.isPM = !endShiftTime.isPM;
            endShiftTime.hour += 12;
        }

        if (endShiftTime.isPM)
        {
            if (endShiftTime.hour == 12)
                endShiftTime.minute = 0;

            endShiftTime.hour = Mathf.Clamp(endShiftTime.hour, 1, 12);
        }

        employmentInformation.endTime = endShiftTime;

        UpdateJobInfo();
        ReloadShiftTime();
    }

    private void ReloadWorkingDays()
    {
        for (int i = 0; i < workingDays.Count; i++)
        {
            DayOfWeek dayOfWeek = (DayOfWeek)i;
            workingDays[i].isOn = employmentInformation.workDays.Contains(dayOfWeek);
        }
    }

    private void ReloadShiftTime()
    {
        ShiftTime startShiftTime = employmentInformation.startTime;
        ShiftTime endShiftTime = employmentInformation.endTime;
        startTimeText.text = $"{startShiftTime.hour}:{startShiftTime.minute.ToString("00")} {(startShiftTime.isPM ? "PM" : "AM")}";
        endTimeText.text = $"{endShiftTime.hour}:{endShiftTime.minute.ToString("00")} {(endShiftTime.isPM ? "PM" : "AM")}";

        MinMaxSlider.MinMaxValues shiftTimeSliderValues = shiftTimeSlider.Values;
        shiftTimeSliderValues.minLimit = 0f;
        shiftTimeSliderValues.maxLimit = GameManager.dayLength * 60f;
        shiftTimeSliderValues.minValue = GameManager.Instance.TimeToSeconds(startShiftTime.hour, startShiftTime.minute, startShiftTime.isPM);
        shiftTimeSliderValues.maxValue = GameManager.Instance.TimeToSeconds(endShiftTime.hour, endShiftTime.minute, endShiftTime.isPM);
        shiftTimeSlider.SetValues(shiftTimeSliderValues, true);
    }

    private void UpdateJobInfo()
    {
        jobManager.holdingJobs[employmentInformation.job] = employmentInformation;
    }
}
