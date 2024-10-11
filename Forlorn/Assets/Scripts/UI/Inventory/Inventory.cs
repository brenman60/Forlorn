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
            if (value >= slots.Count)
                currentSlotIndex_ = 0;
            else if (value < 0)
                currentSlotIndex_ = slots.Count - 1;
            else
                currentSlotIndex_ = value;

            currentSlotIndex_ = Mathf.Clamp(currentSlotIndex_, 0, slots.Count == 0 ? 0 : slots.Count - 1);
            ResetSlotsUI();
        }
    }
    private static int currentSlotIndex_ = 0;

    private int maxSlotSpace
    {
        get
        {
            return (int)RunManager.Instance.statManager.stats[StatType.InventoryMax].currentValue;
        }
    }

    private float slotIndexCooldown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetSlotsUI();
        Keybinds.Instance.controlUse.performed += ItemUse;
    }

    private void OnDestroy()
    {
        Keybinds.Instance.controlUse.performed -= ItemUse;
    }

    private void ItemUse(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (slots.Count > 0 && !GameManager.paused && UIManager.GetUIsOnMouse().Count == 0)
            UseItem();
    }

    private void Update()
    {
        slotIndexCooldown -= Time.deltaTime;
        float slotIndexChange = Keybinds.Instance.controlInventory.ReadValue<float>();
        if (slotIndexCooldown <= 0 && slotIndexChange != 0)
        {
            slotIndexCooldown = 0.15f;
            currentSlotIndex -= Mathf.RoundToInt(Mathf.Clamp(-slotIndexChange, -1, 1));
        }

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

    public bool HasAnySpaceLeft(Item item)
    {
        bool spaceLeft = false;
        if (HasItem(item) == 0)
            spaceLeft = true;
        else
        {
            foreach (KeyValuePair<Item, int> slot in slots)
                if (slot.Key == item && slot.Value < maxSlotSpace)
                    spaceLeft = true;
        }

        return spaceLeft;
    }

    public int HasItem(Item item)
    {
        foreach (KeyValuePair<Item, int> slot in slots)
            if (slot.Key == item)
                return slot.Value;

        return 0;
    }

    public void PutItem(Item item, int amount)
    {
        int currentAmount = HasItem(item);
        if (currentAmount + amount > maxSlotSpace)
        {
            Vector2 dropVelocity = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 2f);
            ItemDropManager.Instance.CreateDrop(item, (currentAmount + amount) - maxSlotSpace, Player.Instance.transform.position, dropVelocity);
        }

        if (currentAmount > 0)
        {
            for (int i = 0; i < slots.Count; i++)
                if (slots[i].Key == item)
                    slots[i] = new KeyValuePair<Item, int>(item, Mathf.Clamp(slots[i].Value + amount, 0, maxSlotSpace));
        }
        else
            slots.Add(new KeyValuePair<Item, int>(item, Mathf.Clamp(amount, 0, maxSlotSpace)));

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

        currentSlotIndex = currentSlotIndex_;

        if (selectedItem.useSound != null)
            SoundManager.Instance.PlayAudio(selectedItem.useSound, true, selectedItem.useSoundVolume);

        ResetSlotsUI();
    }

    public void TakeItem(Item item, int amount)
    {
        int slotIndex = -1;
        for (int i = 0; i < slots.Count; i++)
            if (slots[i].Key == item)
            {
                slotIndex = i;
                break;
            }

        if (slotIndex == -1) return;
        slots[slotIndex] = new KeyValuePair<Item, int>(item, slots[slotIndex].Value - amount);
        if (slots[slotIndex].Value <= 0)
            slots.RemoveAt(slotIndex);

        currentSlotIndex = currentSlotIndex_;
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
