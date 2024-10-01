using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image timeIcon;
    [SerializeField] private Image dayIcon;
    [SerializeField] private List<Sprite> dayIcons;

    private void Update()
    {
        SetTimeText();
        SetTimeIcon();
        SetDayIcon();
    }

    private void SetTimeText()
    {
        float totalHours = (GameManager.Instance.gameTime / (GameManager.dayLength * 60)) * 24f; // (600 / 1200) * 24 = 12 hours
        float currentHour = Mathf.FloorToInt(totalHours);
        float currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60); // (12.5 - 12) * 60 = 30 minutes

        string period = "AM";
        if (currentHour >= 12)
        {
            period = "PM";
            if (currentHour > 12)
                currentHour -= 12;
        }
        else if (currentHour == 0) 
            currentHour = 12;

        timeText.text = currentHour + ":" + currentMinute.ToString("00") + " " + period;
    }

    private void SetTimeIcon()
    {
        Color newColor = Color.Lerp(Color.white, Color.red, GameManager.Instance.gameTime / (GameManager.dayLength * 60));
        timeIcon.color = new Color(newColor.r, newColor.g, newColor.b, timeIcon.color.a);
    }

    private void SetDayIcon()
    {
        switch (GameManager.Instance.dayStatus)
        {
            case DayStatus.Morning:
                dayIcon.sprite = dayIcons[0];
                break;
            case DayStatus.Midday:
                dayIcon.sprite = dayIcons[1];
                break;
            case DayStatus.Night:
                dayIcon.sprite = dayIcons[2];
                break;
        }
    }
}
