using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveData
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private Items items;
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
            if (slots.Count == 0) AddSlots(1);
            PutItem(items.GetItemByName("WaterBottleStale"), 1);
        }

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
        slotsUIs[slotNumber].ChangeItem(slots[slotNumber].Key, slots[slotNumber].Value);
    }

    public void AddSlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int slotNumber = slots.Count + 1;
            slots.Add(slotNumber, new KeyValuePair<Item, int>(null, 0));
            CreateSlot(slotNumber);
        }
    }

    public void PutItem(Item item, int amount)
    {
        // Find free slot and put it in (no empty slots will probably be handled outside of this so we shouldn't worry about it)
        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
            if (slot.Value.Key == null)
            {
                slots[slot.Key] = new KeyValuePair<Item, int>(item, amount);
                slotsUIs[slot.Key].ChangeItem(item, slots[slot.Key].Value);
                return;
            }
            else if (slot.Value.Key == item)
            {
                slots[slot.Key] = new KeyValuePair<Item, int>(item, slots[slot.Key].Value + amount);
                slotsUIs[slot.Key].ChangeItem(item, slots[slot.Key].Value);
                return;
            }
    }

    public void TakeItem(int slotNumber, int amount)
    {
        slots[slotNumber] = new KeyValuePair<Item, int>(slots[slotNumber].Key, slots[slotNumber].Value - amount);
        if (slots[slotNumber].Value <= 0)
            slots.Remove(slotNumber);
    }

    private SlotUI CreateSlot(int slotNumber)
    {
        GameObject newSlot = Instantiate(slotTemplate, slotTemplate.transform.parent);
        newSlot.name = "Slot" + slotNumber;
        SlotUI slotUI = newSlot.GetComponent<SlotUI>();
        newSlot.SetActive(true);

        slotsUIs.Add(slotNumber, slotUI);
        return slotUI;
    }

    public string GetSaveData()
    {
        Dictionary<int, KeyValuePair<string, int>> slotsSaveable = new Dictionary<int, KeyValuePair<string, int>>();
        foreach (KeyValuePair<int, KeyValuePair<Item, int>> slot in slots)
            slotsSaveable.Add(slot.Key, new KeyValuePair<string, int>(slot.Value.Key.name, slot.Value.Value));

        return JsonConvert.SerializeObject(slotsSaveable);
    }

    public void PutSaveData(string data)
    {
        Dictionary<int, KeyValuePair<string, int>> savedSlots = JsonConvert.DeserializeObject<Dictionary<int, KeyValuePair<string, int>>>(data);
        foreach (KeyValuePair<int, KeyValuePair<string, int>> slot in savedSlots)
            slots.Add(slot.Key, new KeyValuePair<Item, int>(items.GetItemByName(slot.Value.Key), slot.Value.Value));

        ResetSlotsUI();
    }
}
