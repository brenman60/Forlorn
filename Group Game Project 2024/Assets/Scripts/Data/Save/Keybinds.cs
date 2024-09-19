using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Keybinds : MonoBehaviour, ISaveData
{
    public static Keybinds Instance { get; private set; }

    private static bool saveLoaded = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject keybindObject = Instantiate(Resources.Load<GameObject>("Utils/Keybinds"));
            keybindObject.name = "Keybinds";

            Keybinds keybinds = keybindObject.GetComponent<Keybinds>();
            Instance = keybinds;

            DontDestroyOnLoad(keybindObject);

            keybinds.ResetAllBinds();
        }
    }

    public readonly Dictionary<KeyType, KeyCode> defaultBinds = new Dictionary<KeyType, KeyCode>()
    {
        [KeyType.Pause] = KeyCode.Escape,
        [KeyType.CutsceneSkip] = KeyCode.Space,
        [KeyType.Left] = KeyCode.A,
        [KeyType.Right] = KeyCode.D,
        [KeyType.Jump] = KeyCode.Space,
        [KeyType.Sprint] = KeyCode.LeftShift,
        [KeyType.Interact] = KeyCode.E,
        [KeyType.Inventory] = KeyCode.Tab,
    };

    public Dictionary<KeyType, KeyCode> Binds { get; private set; } = new Dictionary<KeyType, KeyCode>()
    {

    };

    public static KeyCode GetKeybind(KeyType key)
    {
        if (saveLoaded)
            return Instance.Binds[key];
        else
            return Instance.defaultBinds[key];
    }

    public void SetKeybind(KeyType key, KeyCode newKeyCode)
    {
        Binds[key] = newKeyCode;

        SaveSystem.SaveGlobal();
    }

    public void ResetKeybind(KeyType key)
    {
        Binds[key] = defaultBinds[key];

        SaveSystem.SaveGlobal();
    }

    public void ResetAllBinds()
    {
        Binds = defaultBinds;
    }

    public string GetSaveData()
    {
        string compiledBinds = JsonConvert.SerializeObject(Binds);
        return compiledBinds;
    }

    // Something notable here is that the data will not add any keybinds that were present in the save but missing in the current list of keybinds (it will pretty much discard them)
    public void PutSaveData(string data)
    {
        ResetAllBinds();

        Dictionary<KeyType, KeyCode> savedBinds = JsonConvert.DeserializeObject<Dictionary<KeyType, KeyCode>>(data);
        foreach (KeyValuePair<KeyType, KeyCode> bind in savedBinds)
            if (Binds.ContainsKey(bind.Key))
                Binds[bind.Key] = bind.Value;

        saveLoaded = true;
    }
}

public enum KeyType
{
    Pause,
    Left,
    Right,
    Jump,
    Sprint,
    CutsceneSkip,
    Interact,
    Inventory,
}
