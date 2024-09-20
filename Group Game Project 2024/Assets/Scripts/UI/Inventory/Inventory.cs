using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveData
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private Items items;
    [SerializeField] private int maxSlots;
    [Space(15), SerializeField] private float openSpeed = 15;
    [SerializeField] private GameObject slotTemplate;

    private CanvasGroup canvasGroup;

    // Dictionary storing slot information
    // Key is the slot number, while the value is the slot info
    // Slot info is the item present and how many of it
    private Dictionary<int, KeyValuePair<Item, int>> slots = new Dictionary<int, KeyValuePair<Item, int>>();
    private Dictionary<int, SlotUI> slotsUIs = new Dictionary<int, SlotUI>();

    private bool open;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Keybinds.GetKeybind(KeyType.Inventory)))
            Toggle();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddSlots(1);
            //PutItem(items.GetItemByName("WaterBottleStale"), 1);
        }

        if (Input.GetKeyDown(KeyCode.R))
            PutItem(items.GetItemByName("TornBandages"), 1);

        UpdateCanvasGroup();
    }

    private void UpdateCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, open ? 1f : 0f, Time.deltaTime * openSpeed);
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }

    public void Toggle()
    {
        open = !open;
        if (slots.Count == 0)
            open = false;
    }

    public void ResetSlotsUI()
    {
        slotsUIs.Clear();
        foreach (Transform previousSlot in slotTemplate.transform.parent) if (previousSlot.gameObject != slotTemplate) Destroy(previousSlot.gameObject);

        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
        {
            SlotUI slotUI = CreateSlot(slot.Key);
            ChangeSlotUI(slot.Key);
        }
    }

    public void ChangeSlotUI(int slotNumber)
    {
        if (slots.ContainsKey(slotNumber))
            slotsUIs[slotNumber].ChangeItem(slotNumber, slots[slotNumber].Key, slots[slotNumber].Value);
        else
            slotsUIs[slotNumber].ChangeItem(slotNumber, null, 0);
    }

    public void AddSlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int slotNumber = slotsUIs.Count + 1;
            if (slotNumber > maxSlots) return;

            slots.Add(slotNumber, new KeyValuePair<Item, int>(null, 0));
            CreateSlot(slotNumber);
        }
    }

    public bool CanPutItem(Item item)
    {
        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
            if (slot.Value.Key == item || slot.Value.Key == null)
                return true;

        return false;
    }

    public void PutItem(Item item, int amount)
    {
        // Find free slot and put it in (no empty slots will probably be handled outside of this so we shouldn't worry about it)
        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
            if (slot.Value.Key == null)
            {
                slots[slot.Key] = new KeyValuePair<Item, int>(item, amount);
                slotsUIs[slot.Key].ChangeItem(slot.Key, item, slots[slot.Key].Value);
                return;
            }
            else if (slot.Value.Key == item)
            {
                slots[slot.Key] = new KeyValuePair<Item, int>(item, slots[slot.Key].Value + amount);
                slotsUIs[slot.Key].ChangeItem(slot.Key, item, slots[slot.Key].Value);
                return;
            }
    }

    public void TakeItem(int slotNumber, int amount)
    {
        if (slots[slotNumber].Value <= 0) return;

        slots[slotNumber] = new KeyValuePair<Item, int>(slots[slotNumber].Key, slots[slotNumber].Value - amount);
        if (slots[slotNumber].Value <= 0)
        {
            slots[slotNumber] = new KeyValuePair<Item, int>(null, 0);
            slotsUIs[slotNumber].ChangeItem(slotNumber, null, 0);
        }
    }

    private SlotUI CreateSlot(int slotNumber)
    {
        GameObject newSlot = Instantiate(slotTemplate, slotTemplate.transform.parent);
        newSlot.name = "Slot" + slotNumber;
        SlotUI slotUI = newSlot.GetComponent<SlotUI>();
        slotUI.ChangeItem(slotNumber, null, 0);
        newSlot.SetActive(true);

        slotsUIs.Add(slotNumber, slotUI);
        return slotUI;
    }

    public string GetSaveData()
    {
        Dictionary<int, KeyValuePair<string, int>> slotsSaveable = new Dictionary<int, KeyValuePair<string, int>>();
        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
        {
            string itemName = slot.Value.Key != null ? slot.Value.Key.name : string.Empty;
            slotsSaveable.Add(slot.Key, new KeyValuePair<string, int>(itemName, slot.Value.Value));
        }

        return JsonConvert.SerializeObject(slotsSaveable);
    }

    public void PutSaveData(string data)
    {
        Dictionary<int, KeyValuePair<string, int>> savedSlots = JsonConvert.DeserializeObject<Dictionary<int, KeyValuePair<string, int>>>(data);
        foreach (KeyValuePair<int, KeyValuePair<string, int>> slot in savedSlots)
        {
            Item item = !string.IsNullOrEmpty(slot.Value.Key) ? items.GetItemByName(slot.Value.Key) : null;
            slots.Add(slot.Key, new KeyValuePair<Item, int>(item, slot.Value.Value));
        }

        ResetSlotsUI();
    }
}
