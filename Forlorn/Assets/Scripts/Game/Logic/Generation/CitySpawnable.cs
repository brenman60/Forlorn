using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City/New Spawnable", fileName = "New Spawnable")]
public class CitySpawnable : ScriptableObject
{
    public GameObject spawnObject;
    [Tooltip("Determines the rarity of each instance of this spawnable. Index 0 = 1st, Index 1 = 2nd, etc.")] 
    public List<SpawnAmount> spawnAmounts = new List<SpawnAmount>()
    {
        new SpawnAmount(1),
    };

    [Header("Position Data")]
    public Vector3 minPosition = Vector3.zero;
    public Vector3 maxPosition = Vector3.zero;
    public bool equalPositionDistribution = false;

    [Header("Scale Data")]
    public Vector3 minScale = Vector3.one;
    public Vector3 maxScale = Vector3.one;
    [Tooltip("Syncs every coordinate vector with the X value.")] public bool linkedScales = true;
    [Tooltip("Divides Y position by Y scale.")] public bool scaleEffectsHeight;

    [Serializable]
    public struct SpawnAmount
    {
        [Range(0, 1)] public float rarity;

        public SpawnAmount(float rarity)
        {
            this.rarity = rarity;
        }
    }
}
