using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveData
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private Items items;
    [Space(15), SerializeField] private SlotUI previousSlot;
    [SerializeField] private SlotUI currentSlot;
    [SerializeField] private SlotUI nextSlot;
    [Space(15), SerializeField] private CanvasGroup namePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [Space(15), SerializeField] private CanvasGroup amountPanel;
    [SerializeField] private TextMeshProUGUI amountText;

    private static List<KeyValuePair<Item, int>> slots = new List<KeyValuePair<Item, int>>();
    private int currentSlotIndex
    {
        get { return currentSlotIndex_; }
        set
        {
            currentSlotIndex_ = Mathf.Clamp(value, 0, slots.Count == 0 ? 0 : slots.Count - 1);
            ResetSlotsUI();
        }
    }
    private static int currentSlotIndex_ = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetSlotsUI();
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0f)
            currentSlotIndex += Mathf.RoundToInt(Input.mouseScrollDelta.y);

        if (slots.Count > 0 && Input.GetKeyDown(Keybinds.GetKeybind(KeyType.ItemUse)) && !GameManager.paused && UIManager.GetUIsOnMouse().Count == 0)
            UseItem();

        UpdatePanels();

        if (DeathUI.PlayerDead) enabled = false;
    }

    private void UpdatePanels()
    {
        bool panelsVisible = slots.Count > 0;
        namePanel.alpha = Mathf.Lerp(namePanel.alpha, panelsVisible ? 1f : 0f, Time.deltaTime * 20f);
        amountPanel.alpha = Mathf.Lerp(amountPanel.alpha, panelsVisible ? 1f : 0f, Time.deltaTime * 20f);
    }

    public void ResetSlotsUI()
    {
        bool previousValid = currentSlotIndex - 1 != -1;
        bool currentValid = slots.Count > 0;
        bool nextValid = currentSlotIndex + 1 < slots.Count;

        previousSlot.ChangeItem(previousValid ? slots[currentSlotIndex - 1].Key : null);
        currentSlot.ChangeItem(currentValid ? slots[currentSlotIndex].Key : null);
        nextSlot.ChangeItem(nextValid ? slots[currentSlotIndex + 1].Key : null);

        if (currentValid)
        {
            nameText.text = slots[currentSlotIndex].Key.visibleName;
            amountText.text = slots[currentSlotIndex].Value.ToString();
        }
    }

    public bool HasItem(Item item)
    {
        foreach (KeyValuePair<Item, int> slot in slots)
            if (slot.Key == item)
                return true;

        return false;
    }

    public void PutItem(Item item, int amount)
    {
        if (HasItem(item))
        {
            for (int i = 0; i < slots.Count; i++)
                if (slots[i].Key == item)
                    slots[i] = new KeyValuePair<Item, int>(item, slots[i].Value + amount);
        }
        else
            slots.Add(new KeyValuePair<Item, int>(item, amount));

        ResetSlotsUI();
    }

    public void UseItem()
    {
        Item selectedItem = slots[currentSlotIndex].Key;
        if (selectedItem.useStats != null)
            foreach (UseStat useStats in selectedItem.useStats)
                RunManager.Instance.statManager.stats[useStats.targetStat].currentValue += useStats.change;

        if (selectedItem.useEffects != null)
            foreach (UseEffect effect in selectedItem.useEffects)
                RunManager.Instance.statManager.ApplyEffect((Effect)Activator.CreateInstance(effect.effect.Type, RunManager.Instance.statManager, effect.saveable));

        if (selectedItem.useModifiers != null)
            foreach (UseModifier modifier in selectedItem.useModifiers)
                RunManager.Instance.statManager.ApplyModifier(new StatModifier(RunManager.Instance.statManager.stats[modifier.targetStat], modifier.modifierAmount, modifier.isExponential));

        slots[currentSlotIndex] = new KeyValuePair<Item, int>(slots[currentSlotIndex].Key, slots[currentSlotIndex].Value - 1);
        if (slots[currentSlotIndex].Value <= 0)
            slots.RemoveAt(currentSlotIndex);

        if (selectedItem.useSound != null)
            SoundManager.Instance.PlayAudio(selectedItem.useSound, true, selectedItem.useSoundVolume);

        ResetSlotsUI();
    }

    public string GetSaveData()
    {
        Dictionary<string, int> slotsSaveable = new Dictionary<string, int>();
        foreach (KeyValuePair<Item, int> slot in slots)
            slotsSaveable.Add(slot.Key.name, slot.Value);

        string[] dataPoints = new string[2]
        {
            JsonConvert.SerializeObject(slotsSaveable),
            currentSlotIndex.ToString(),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    public void PutSaveData(string data)
    {
        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);

        slots.Clear();
        Dictionary<string, int> savedSlots = JsonConvert.DeserializeObject<Dictionary<string, int>>(dataPoints[0]);
        foreach (KeyValuePair<string, int> slot in savedSlots)
        {
            Item item = items.GetItemByName(slot.Key);
            slots.Add(new KeyValuePair<Item, int>(item, slot.Value));
        }

        currentSlotIndex = int.Parse(dataPoints[1]);

        ResetSlotsUI();
    }
}
