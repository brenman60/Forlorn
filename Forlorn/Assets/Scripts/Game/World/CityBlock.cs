using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityBlock : MonoBehaviour, ISaveData
{
    public float length;

    [SerializeField] private Transform spawnablesParent;

    // Spawnables will be things like backgrounds, foregrounds, etc. They should be pretty modular I think, as their spawn position offsets will be local position dependant, allowing for customization from the parent transform itself.
    [SerializeField] protected CitySpawnable[] spawnables;
    private List<GameObject> spawnedSpawnables = new List<GameObject>();

    protected virtual void InitSpawnables()
    {
        Dictionary<CitySpawnable, int> selectedSpawnables = new Dictionary<CitySpawnable, int>();
        foreach (CitySpawnable spawnable in spawnables)
        {
            int amount = 0;
            for (int i = 0; i < spawnable.spawnAmounts.Count; i++)
                if (spawnable.spawnAmounts[i].rarity >= Random.Range(0f, 1f))
                    amount++;

            if (amount > 0)
                selectedSpawnables.Add(spawnable, amount);
        }

        foreach (KeyValuePair<CitySpawnable, int> spawnable in selectedSpawnables)
        {
            for (int spawn = 0; spawn < spawnable.Value; spawn++)
            {
                Vector3 minPos = spawnable.Key.minPosition;
                Vector3 maxPos = spawnable.Key.maxPosition;

                Vector3 minScale = spawnable.Key.minScale;
                Vector3 maxScale = spawnable.Key.maxScale;

                float distributeInterpolation = spawn / Mathf.Clamp(spawnable.Value - 1, 1, float.MaxValue);
                Vector3 spawnPosition = !spawnable.Key.equalPositionDistribution ? 
                    new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z)) : 
                    Vector3.Lerp(spawnable.Key.minPosition, spawnable.Key.maxPosition, distributeInterpolation);

                float linkedScale = Random.Range(minScale.x, maxScale.x);
                Vector3 spawnScale = !spawnable.Key.linkedScales ?
                    new Vector3(Random.Range(minScale.x, maxScale.x), Random.Range(minScale.y, maxScale.y), Random.Range(minScale.z, maxScale.z)) :
                    new Vector3(linkedScale, linkedScale, linkedScale);

                if (spawnable.Key.scaleEffectsHeight)
                    spawnPosition.y /= spawnScale.y;

                InstantiateSpawn(spawnable.Key,
                        spawnPosition,
                        spawnScale,
                        Quaternion.identity);
            }
        }
    }

    protected GameObject InstantiateSpawn(CitySpawnable citySpawnable, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        GameObject spawnObject = Instantiate(citySpawnable.spawnObject, spawnablesParent);
        spawnObject.name = citySpawnable.spawnObject.name;
        spawnObject.transform.localPosition = position;
        spawnObject.transform.localScale = scale;
        spawnObject.transform.rotation = rotation;
        spawnObject.SetActive(true);

        spawnedSpawnables.Add(spawnObject);
        return spawnObject;
    }

    public virtual string GetSaveData()
    {
        string[] blockInformation = new string[1]
        {
            transform.position.ToJSON(),
        };

        List<string> spawnables = new List<string>();
        foreach (GameObject spawn in spawnedSpawnables)
        {
            string[] spawnData = new string[5]
            {
                spawn.name,
                spawn.transform.localPosition.ToJSON(),
                spawn.transform.localScale.ToJSON(),
                spawn.transform.rotation.ToJSON(),
                spawn.TryGetComponent(out ISaveData saveData) ? saveData.GetSaveData() : string.Empty,
            };

            spawnables.Add(JsonConvert.SerializeObject(spawnData));
        }

        string[] dataPoints = new string[2]
        {
            JsonConvert.SerializeObject(blockInformation),
            JsonConvert.SerializeObject(spawnables),
        };

        return JsonConvert.SerializeObject(dataPoints);
    }

    // Sections saving and loading will be kind of weird
    // As a baseline, PutSaveData should essentially be treated as Start (or rather Awake probably), when talking about changing the Section's data or look.
    // Like InitSpawnables for example, should not be called in Start because it would probably be better to save the backgrounds spawned and load them, rather than regenerate them.
    public virtual void PutSaveData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            InitSpawnables();
            return;
        }

        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(data);

        string[] blockInformation = JsonConvert.DeserializeObject<string[]>(dataPoints[0]);
        transform.position = blockInformation[0].ToVector3();

        List<string> savedSpawnables = JsonConvert.DeserializeObject<List<string>>(dataPoints[1]);
        foreach (string spawnDataRaw in savedSpawnables)
        {
            string[] spawnData = JsonConvert.DeserializeObject<string[]>(spawnDataRaw);
            string spawnName = spawnData[0];
            Vector3 positionData = spawnData[1].ToVector3();
            Vector3 scaleData = spawnData[2].ToVector3();
            Quaternion rotationData = spawnData[3].ToQuaternion();
            string savedData = spawnData[4];

            CitySpawnable citySpawnable = spawnables[0];
            foreach (CitySpawnable citySpawnableData in spawnables)
                if (citySpawnableData.spawnObject.name == spawnName)
                {
                    citySpawnable = citySpawnableData;
                    break;
                }

            GameObject spawnedObject = InstantiateSpawn(citySpawnable, positionData, scaleData, rotationData);
            if (spawnedObject.TryGetComponent(out ISaveData saveData) && !string.IsNullOrEmpty(savedData))
                saveData.PutSaveData(savedData);
        }
    }
}
