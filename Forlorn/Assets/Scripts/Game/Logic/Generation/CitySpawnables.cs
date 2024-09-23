using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/City Spawnables", fileName = "City Spawnables")]
public class CitySpawnables : ScriptableObject
{
    public List<CitySpawnable> spawnables = new List<CitySpawnable>();

    public CitySpawnable GetSpawnableByName(string name)
    {
        foreach (CitySpawnable spawnable in spawnables)
            if (spawnable.name == name)
                return spawnable;

        Debug.LogError("Spawnable with name '" + name + "'. Not found.");
        return null;
    }
}
