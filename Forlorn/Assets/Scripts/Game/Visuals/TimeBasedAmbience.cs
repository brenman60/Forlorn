using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeBasedAmbience : MonoBehaviour
{
    [SerializeField] private List<TimeAmbience> timeAmbiences = new List<TimeAmbience>();

    private void Start()
    {
        GameManager.Instance.dayStatusChanged += DayStatusChanged;
        DayStatusChanged(null, GameManager.Instance.dayStatus);
    }

    private void OnDestroy()
    {
        GameManager.Instance.dayStatusChanged -= DayStatusChanged;
    }

    private void DayStatusChanged(object sender, DayStatus newStatus)
    {
        foreach (TimeAmbience timeAmbience in timeAmbiences)
        {
            bool isTime = newStatus == timeAmbience.status;
            if (timeAmbience.statusObjects != null)
                foreach (ParticleSystem timeObject in timeAmbience.statusObjects)
                {
                    if (isTime && !timeObject.isPlaying)
                        timeObject.Play();
                    else if (timeObject.isPlaying)
                        timeObject.Stop();
                }
        }
    }

    [Serializable]
    public struct TimeAmbience
    {
        public DayStatus status;
        public List<ParticleSystem> statusObjects;
    }
}
