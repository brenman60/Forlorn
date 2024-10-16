using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shops/New Shop List", fileName = "New Shop List")]
public class Shops : ScriptableObject
{
    public List<Shop> shops = new List<Shop>();

    public Shop GetShopByName(string name)
    {
        foreach (Shop shop in shops)
            if (shop.name == name)
                return shop;

        Debug.LogError("Shop with name '" + name + "' not found. Returning null.");
        return null;
    }
}
