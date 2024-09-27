using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour, ISaveData
{
    public static Keybinds Instance { get; private set; }

    public static PlayerInput playerControls { get; private set; }
    public static InputAction controlMove { get; private set; }

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

            playerControls = new PlayerInput();
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
        [KeyType.ItemUse] = KeyCode.Mouse0,
        //[KeyType.ItemUse] = KeyCode.Q,
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

    private void OnEnable()
    {
        if (playerControls != null)
        {
            controlMove = playerControls.Player.Movement;
            controlMove.Enable();
        }
    }

    private void OnDisable()
    {
        if (controlMove != null)
            controlMove.Disable();
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
    ItemUse,
}
