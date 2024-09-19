using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item List", fileName = "Item List")]
public class Items : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public Item GetItemByName(string name)
    {
        foreach (Item item in items)
            if (item.name == name)
                return item;

        Debug.LogError("Item not '" + name + "' found. Returning null.");
        return null;
    }
}
