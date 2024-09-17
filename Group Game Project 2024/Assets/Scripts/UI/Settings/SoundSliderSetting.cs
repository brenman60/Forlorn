using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderSetting : MonoBehaviour
{
    [SerializeField] private SoundType soundType;
    [Space(15), SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percentageText;

    private void Start()
    {
        StartCoroutine(WaitForLoaded());
    }

    private IEnumerator WaitForLoaded()
    {
        yield return new WaitUntil(() => GameSettings.saveLoaded);
        Dictionary<SoundType, float> soundVolumes = GameSettings.GetSetting<Dictionary<SoundType, float>>(SettingType.Audio);
        slider.value = soundVolumes[soundType];
        SetPercentageText();
    }

    public void OnVolumeChanged()
    {
        Dictionary<SoundType, float> soundVolumes = GameSettings.GetSetting<Dictionary<SoundType, float>>(SettingType.Audio);
        soundVolumes[soundType] = slider.value;
        GameSettings.Instance.SetSetting(SettingType.Audio, soundVolumes);

        SetPercentageText();
    }

    private void SetPercentageText()
    {
        percentageText.text = Mathf.RoundToInt(slider.value * 100) + "%";
    }
}
