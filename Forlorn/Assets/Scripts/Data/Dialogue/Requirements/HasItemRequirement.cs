using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Requirements/Has Items", fileName = "Has Items")]
public class HasItemRequirement : DialogueRequirement
{
    [SerializeField] private List<ItemRequirement> requirements = new List<ItemRequirement>();

    public override bool MeetsRequirement()
    {
        bool hasItems = true;

        Inventory inventory = Inventory.Instance;
        foreach (ItemRequirement requirement in requirements)
        {
            int currentAmount = inventory.HasItem(requirement.item);
            if (currentAmount < requirement.amount)
                hasItems = false;
        }

        return hasItems;
    }

    public void RemoveRequirements()
    {
        Inventory inventory = Inventory.Instance;
        foreach (ItemRequirement requirement in requirements)
        {
            if (!requirement.takesAmount) continue;
            inventory.TakeItem(requirement.item, requirement.amount);
        }
    }

    [Serializable]
    protected struct ItemRequirement
    {
        public Item item;
        public int amount;
        public bool takesAmount;
    }
}
