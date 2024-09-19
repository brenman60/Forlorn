using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/New Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string visibleName;
}
