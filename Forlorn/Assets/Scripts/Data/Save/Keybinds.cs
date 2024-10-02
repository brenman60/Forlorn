using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour, ISaveData
{
    public static Keybinds Instance { get; private set; }

    public PlayerInput playerControls { get; private set; }
    public InputAction controlMove { get; private set; }
    public InputAction controlRun { get; private set; }
    public InputAction controlInteract { get; private set; }
    public InputAction controlUse { get; private set; }
    public InputAction controlInventory { get; private set; }
    public InputAction controlUIDrag { get; private set; }
    public InputAction controlSkipCutscene { get; private set; }

    private static bool saveLoaded = false;

    private float saveTimer;

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
        [KeyType.ItemUse] = KeyCode.Mouse0,
        //[KeyType.ItemUse] = KeyCode.Q,
        [KeyType.SkillsUIDrag] = KeyCode.Mouse1,
        [KeyType.SkillsUIOpen] = KeyCode.Tab,
    };

    public Dictionary<KeyType, KeyCode> Binds { get; private set; } = new Dictionary<KeyType, KeyCode>()
    {

    };

    private void Update()
    {
        if (saveTimer > 0)
        {
            saveTimer -= Time.deltaTime;
            if (saveTimer <= 0)
                SaveSystem.SaveGlobal();
        }
    }

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

        saveTimer = 0.25f;
    }

    public void ResetKeybind(KeyType key)
    {
        Binds[key] = defaultBinds[key];

        saveTimer = 0.25f;
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
        foreach (InputDevice device in InputSystem.devices)
            InputSystem.EnableDevice(device);

        playerControls = new PlayerInput();

        controlMove = playerControls.Player.Movement;
        controlMove.Enable();

        controlRun = playerControls.Player.Run;
        controlRun.Enable();

        controlInteract = playerControls.Player.Interact;
        controlInteract.Enable();

        controlUse = playerControls.Player.Use;
        controlUse.Enable();

        controlUIDrag = playerControls.UI.UIDrag;
        controlUIDrag.Enable();

        controlInventory = playerControls.UI.Inventory;
        controlInventory.Enable();

        controlSkipCutscene = playerControls.UI.SkipCutscene;
        controlSkipCutscene.Enable();
    }

    private void OnDisable()
    {
        controlMove?.Disable();
        controlRun?.Disable();
        controlInteract?.Disable();
        controlUse?.Disable();
        controlUIDrag?.Disable();
        controlInventory?.Disable();
        controlSkipCutscene?.Disable();
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
    SkillsUIDrag,
    SkillsUIOpen,
}
