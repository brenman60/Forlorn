using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    [SerializeField] private float defaultVolume = 0.25f;
    [SerializeField] private List<TimeBasedMusic> musicSelections = new List<TimeBasedMusic>();

    private AudioSource audioSource;

    private Coroutine changeTrack;
    private float changeTrackCooldown;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.dayStatusChanged += DayStatusChanged;
        changeTrack = StartCoroutine(ChangeTrack());
    }

    private void Update()
    {
        changeTrackCooldown -= Time.deltaTime;
        if (changeTrackCooldown <= 0f)
        {
            if (changeTrack != null)
                StopCoroutine(changeTrack);

            changeTrack = StartCoroutine(ChangeTrack());
        }
    }

    private void DayStatusChanged(object sender, DayStatus status) => changeTrackCooldown = 0f;

    private IEnumerator ChangeTrack()
    {
        DayStatus status = GameManager.Instance.dayStatus;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime * 0.1f;
            if (audioSource.volume < 0f) audioSource.volume = 0f;

            yield return new WaitForEndOfFrame();
        }

        TimeBasedMusic musicTracks = new TimeBasedMusic();
        foreach (TimeBasedMusic timeBasedMusic in musicSelections)
            if (timeBasedMusic.dayStatus == status)
            {
                musicTracks = timeBasedMusic;
                break;
            }

        AudioClip currentClip = audioSource.clip;
        while (audioSource.clip == currentClip)
        {
            audioSource.clip = musicTracks.tracks[UnityEngine.Random.Range(0, musicTracks.tracks.Count - 1)];
            if (musicTracks.tracks.Count == 1) break;

            yield return new WaitForEndOfFrame();
        }

        audioSource.Play();
        changeTrackCooldown = audioSource.clip.length;

        while (audioSource.volume < defaultVolume)
        {
            audioSource.volume += Time.deltaTime * 0.25f;
            if (audioSource.volume > defaultVolume) audioSource.volume = defaultVolume;

            yield return new WaitForEndOfFrame();
        }
    }

    [Serializable]
    public struct TimeBasedMusic
    {
        public DayStatus dayStatus;
        public List<AudioClip> tracks;
    }
}
