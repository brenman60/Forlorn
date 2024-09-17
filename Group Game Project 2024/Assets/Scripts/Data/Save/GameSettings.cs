using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour, ISaveData
{
    public static GameSettings Instance { get; private set; }
    public static event EventHandler<SettingType> SettingChanged;

    public static bool saveLoaded { get; private set; } = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject settingsObject = Instantiate(Resources.Load<GameObject>("Utils/GameSettings"));
            settingsObject.name = "GameSettings";

            GameSettings settings = settingsObject.GetComponent<GameSettings>();
            Instance = settings;

            DontDestroyOnLoad(settingsObject);

            settings.ResetAllSettings();
        }
    }

    public readonly Dictionary<SettingType, object> defaultSettings = new Dictionary<SettingType, object>()
    {
        [SettingType.InstantPause] = true,
        [SettingType.Audio] = new Dictionary<SoundType, float>() 
        {
            [SoundType.Global] = 1,
            [SoundType.Music] = 1,
            [SoundType.UI] = 1,
            [SoundType.Environment] = 1,
            [SoundType.Game] = 1,
        },
    };

    // SettingType is being used here for better syntax and readability pretty much
    // Could've just used a string for the setting keyword but that can make for issues down the line
    public Dictionary<SettingType, object> Settings { get; private set; } = new Dictionary<SettingType, object>()
    {

    };

    // I love generics
    public static T GetSetting<T>(SettingType setting)
    {
        if (saveLoaded)
            return (T)Instance.Settings[setting];
        else
            return (T)Instance.defaultSettings[setting];
    }

    public void SetSetting(SettingType setting, object newSettingValue)
    {
        if (SettingChanged != null) SettingChanged.Invoke(this, setting);

        Settings[setting] = newSettingValue;

        SaveSystem.SaveGlobal();
    }

    public void ResetSetting(SettingType setting)
    {
        Settings[setting] = defaultSettings[setting];

        SaveSystem.SaveGlobal();
    }

    public void ResetAllSettings()
    {
        Settings = defaultSettings;
    }

    public string GetSaveData()
    {
        string compiledSettings = JsonConvert.SerializeObject(Settings);
        return compiledSettings;
    }

    // Something notable here is that the data will not add any keybinds that were present in the save, but missing in the current list of keybinds (it will pretty much discard them)
    public void PutSaveData(string data)
    {
        ResetAllSettings();

        Dictionary<SettingType, object> savedSettings = JsonConvert.DeserializeObject<Dictionary<SettingType, object>>(data);
        foreach (KeyValuePair<SettingType, object> setting in savedSettings)
            if (Settings.ContainsKey(setting.Key)) 
                Settings[setting.Key] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(setting.Value), defaultSettings[setting.Key].GetType());

        saveLoaded = true;
    }
}

public enum SettingType
{
    InstantPause,
    Audio,
}

public enum SoundType
{
    Global,
    Music,
    UI,
    Environment,
    Game,
}
