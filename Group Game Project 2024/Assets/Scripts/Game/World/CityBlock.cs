using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityBlock : MonoBehaviour, ISaveData
{
    public float length;

    // Spawnables will be things like backgrounds, foregrounds, etc. They should be pretty modular I think, as their spawn position offsets will be local position dependant, allowing for customization from the parent transform itself.
    [SerializeField] protected CitySpawnable[] spawnables;
    private List<GameObject> spawnedSpawnables = new List<GameObject>();

    protected virtual void InitSpawnables()
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            CitySpawnable spawnable = spawnables[i];
            Vector3 posMin = spawnable.spawnPosMin;
            Vector3 posMax = spawnable.spawnPosMax;
            int randomAmount = UnityEngine.Random.Range(spawnable.minAmount, spawnable.maxAmount + 1);
            for (int spawn = 0; spawn < randomAmount; spawn++)
            {
                float distributeInterpolation = spawn / Mathf.Clamp(randomAmount - 1, 1, float.MaxValue);

                Vector2 spawnPosition = !spawnable.distributeEqually ?
                    new Vector3(UnityEngine.Random.Range(posMin.x, posMax.x), UnityEngine.Random.Range(posMin.y, posMax.y), UnityEngine.Random.Range(posMin.z, posMax.z)) :
                    Vector3.Lerp(posMin, posMax, distributeInterpolation);

                InstantiateSpawn(spawnable, 
                    spawnPosition,
                    Vector3.one,
                    Quaternion.identity);
            }
        }
    }

    protected GameObject InstantiateSpawn(CitySpawnable citySpawnable, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        GameObject spawnObject = Instantiate(citySpawnable.spawnObject, citySpawnable.spawnParent);
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

[Serializable]
public struct CitySpawnable
{
    [Header("Spawn Object Data")]
    public GameObject spawnObject;
    public Transform spawnParent;
    public int minAmount, maxAmount;
    [Space(15), Header("Spawn Postion Data")]
    public Vector3 spawnPosMin;
    public Vector3 spawnPosMax;
    [Tooltip("Will spawn the objects at equal distributions between the spawnPosMin and spawnPosMax")]public bool distributeEqually;
}
