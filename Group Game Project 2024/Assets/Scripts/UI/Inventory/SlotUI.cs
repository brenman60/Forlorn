using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [Space(15), SerializeField] private GameObject namePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [Space(15), SerializeField] private GameObject contextPanel;

    private int slotNumber;
    private Item selectedItem;
    private int quantity;

    public void ChangeItem(int newSlotNumber, Item newItem, int newQuantity)
    {
        slotNumber = newSlotNumber;
        selectedItem = newItem;
        quantity = newQuantity;
        itemAmount.text = newQuantity > 0 ? newQuantity.ToString() : string.Empty;

        UpdateItemImage();
        UpdateItemName();

        ToggleNameView(false);
        if (contextPanel.activeSelf) ToggleContextView();
    }

    private void UpdateItemImage()
    {
        if (selectedItem != null)
        {
            itemImage.sprite = selectedItem.icon;
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.sprite = null;
            itemImage.color = new Color(0, 0, 0, 0);
        }
    }

    private void UpdateItemName()
    {
        if (selectedItem != null)
            nameText.text = selectedItem.visibleName;
        else
            nameText.text = string.Empty;
    }

    public void ToggleNameView(bool toggle)
    {
        namePanel.SetActive(toggle);
        if (selectedItem == null)
            namePanel.SetActive(false);
    }

    public void ToggleContextView()
    {
        contextPanel.SetActive(!contextPanel.activeSelf);
        if (selectedItem == null)
            contextPanel.SetActive(false);
    }

    public void Use()
    {
        if (quantity <= 0) return;

        if (selectedItem.useStats != null)
            foreach (UseStat useStats in selectedItem.useStats)
                RunManager.Instance.statManager.stats[useStats.targetStat].currentValue += useStats.change;

        if (selectedItem.useEffects != null)
            foreach (UseEffect effect in selectedItem.useEffects)
                RunManager.Instance.statManager.ApplyEffect((Effect)Activator.CreateInstance(effect.effect.Type, RunManager.Instance.statManager, effect.saveable));

        if (selectedItem.useModifiers != null)
            foreach (UseModifier modifier in selectedItem.useModifiers)
                RunManager.Instance.statManager.ApplyModifier(new StatModifier(RunManager.Instance.statManager.stats[modifier.targetStat], modifier.modifierAmount, modifier.isExponential));

        Inventory.Instance.TakeItem(slotNumber, 1);
        Inventory.Instance.ChangeSlotUI(slotNumber);
    }
}
