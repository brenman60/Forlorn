using UnityEngine;

[CreateAssetMenu(menuName = "City/New Section", fileName = "New Section")]
public class CitySection : ScriptableObject
{
    public GameObject prefabObject;
    [Tooltip("Defines how many sections in a row are required to utilize this section")] public int requiredUpTo;
    public int maxPerSection = 0;
    [Range(0, 1)] public float rarity;
}
