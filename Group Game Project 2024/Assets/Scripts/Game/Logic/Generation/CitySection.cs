using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/New Section", fileName = "New Section")]
public class CitySection : ScriptableObject
{
    public GameObject prefabObject;

    [Header("Section Requirements")]
    public List<string> requiredInSections = new List<string>();
    public bool requiredInAnySection;
    public int maxPerSection = 0;

    [Header("City Requirements")]
    public int requiredInCity = 0;
    public int maxPerCity = 0;

    [Header("Spawn Requirements")]
    [Range(0, 1)] public float rarity = 1;
    public bool cannotNaturallySpawn = false;
}
