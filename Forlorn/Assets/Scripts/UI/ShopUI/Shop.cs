using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shops/New Shop", fileName = "New Shop")]
public class Shop : ScriptableObject
{
    public string visibleName = "Template Shop Name";
    public List<Item> shopItems = new List<Item>();
}
