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
        int originalHour = startShiftTime.hour;
        startShiftTime.minute += minutes;
        if (startShiftTime.minute >= 60)
        {
            startShiftTime.minute -= 60;
            startShiftTime.hour++;
        }
        else if (startShiftTime.minute < 0)
        {
            startShiftTime.minute += 60;
            startShiftTime.hour--;
        }

        startShiftTime.hour = Mathf.Clamp(startShiftTime.hour, 6, employmentInformation.endTime.hour);

        if (MinutesDifference(startShiftTime, employmentInformation.endTime) >= 60)
            employmentInformation.startTime = startShiftTime;

        UpdateJobInfo();
        ReloadShiftTime();
    }

    public void UpdateEndTime(int minutes)
    {
        if (loadingInfo) return;

        ShiftTime endShiftTime = employmentInformation.endTime;
        int originalHour = endShiftTime.hour;
        endShiftTime.minute += minutes;
        if (endShiftTime.minute >= 60)
        {
            endShiftTime.minute -= 60;
            endShiftTime.hour++;
        }
        else if (endShiftTime.minute < 0)
        {
            endShiftTime.minute += 60;
            endShiftTime.hour--;
        }

        endShiftTime.hour = Mathf.Clamp(endShiftTime.hour, employmentInformation.startTime.hour, 23);

        if (MinutesDifference(employmentInformation.startTime, endShiftTime) >= 60)
            employmentInformation.endTime = endShiftTime;

        UpdateJobInfo();
        ReloadShiftTime();
    }

    private int MinutesDifference(ShiftTime startTime, ShiftTime endTime)
    {
        int hoursDifference = Mathf.Abs(startTime.hour - endTime.hour);
        int minutesDifference = Mathf.Abs(startTime.minute - endTime.minute);

        return (hoursDifference * 60) + minutesDifference;
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

        bool startIsPM = startShiftTime.hour > 12;
        bool endIsPM = endShiftTime.hour > 12;

        int startHour = startShiftTime.hour - (startIsPM ? 12 : 0);
        int endHour = endShiftTime.hour - (endIsPM ? 12 : 0);

        startTimeText.text = $"{startHour}:{startShiftTime.minute.ToString("00")} {(startIsPM ? "PM" : "AM")}";
        endTimeText.text = $"{endHour}:{endShiftTime.minute.ToString("00")} {(endIsPM ? "PM" : "AM")}";

        MinMaxSlider.MinMaxValues shiftTimeSliderValues = shiftTimeSlider.Values;
        shiftTimeSliderValues.minLimit = 0f;
        shiftTimeSliderValues.maxLimit = 24f;
        shiftTimeSliderValues.minValue = startShiftTime.hour + (startShiftTime.minute / 100);
        shiftTimeSliderValues.maxValue = endShiftTime.hour + (endShiftTime.minute / 100);
        shiftTimeSlider.SetValues(shiftTimeSliderValues, true);
    }

    private void UpdateJobInfo()
    {
        jobManager.holdingJobs[employmentInformation.job] = employmentInformation;
    }
}
