using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundConfigure : MonoBehaviour
{
    public SoundType soundType;
    private AudioSource audioSource;

    private float initialVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
        audioSource.Play();

        GameSettings.SettingChanged += SettingsChanged;
        RecalculateVolume();

        Destroy(gameObject, audioSource.clip.length);
    }

    private void SettingsChanged(object sender, SettingType changedSetting)
    {
        if (changedSetting != SettingType.Audio) return;

        RecalculateVolume();
    }

    private void RecalculateVolume()
    {
        Dictionary<SoundType, float> soundVolumes = GameSettings.GetSetting<Dictionary<SoundType, float>>(SettingType.Audio);
        float globalVolume = soundVolumes[SoundType.Global];
        float soundVolume = soundVolumes[soundType];

        audioSource.volume = initialVolume * globalVolume * soundVolume;
    }
}
