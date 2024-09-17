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
                InstantiateSpawn(spawnable, 
                    new Vector3(UnityEngine.Random.Range(posMin.x, posMax.x), UnityEngine.Random.Range(posMin.y, posMax.y), UnityEngine.Random.Range(posMin.z, posMax.z)),
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
        List<string> spawnables = new List<string>();
        foreach (GameObject spawn in spawnedSpawnables)
        {
            string[] spawnData = new string[4]
            {
                spawn.name,
                spawn.transform.position.x + " " + spawn.transform.position.y + " " + spawn.transform.position.z,
                spawn.transform.localScale.x + " " + spawn.transform.localScale.y + " " + spawn.transform.localScale.z,
                spawn.transform.rotation.eulerAngles.x + " " + spawn.transform.rotation.eulerAngles.y + " " + spawn.transform.rotation.eulerAngles.z,
            };

            spawnables.Add(JsonConvert.SerializeObject(spawnData));
        }

        return JsonConvert.SerializeObject(spawnables);
    }

    // Sections saving and loading will be kind of weird
    // As a baseline, PutSaveData should essentially be treated as Start (or rather Awake probably), when talking about changing the Section's data or look.
    // Like SpawnBackground for example, should not be called in Start because it would probably be better to save the backgrounds spawned and load them, rather than regenerate them.
    public virtual void PutSaveData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            InitSpawnables();
            return;
        }

        List<string> spawnables = JsonConvert.DeserializeObject<List<string>>(data);
        foreach (string spawnDataRaw in spawnables)
        {
            string[] spawnData = JsonConvert.DeserializeObject<string[]>(spawnDataRaw);
            string spawnName = spawnData[0];
            string[] positionData = spawnData[1].Split(' ');
            string[] scaleData = spawnData[2].Split(' ');
            string[] rotationData = spawnData[3].Split(' ');

            CitySpawnable citySpawnObject = this.spawnables[0];
            foreach (CitySpawnable spawnableData in this.spawnables)
                if (spawnableData.spawnObject.name == spawnName)
                {
                    citySpawnObject = spawnableData;
                    break;
                }

            GameObject spawnObject = InstantiateSpawn(citySpawnObject,
                new Vector3(float.Parse(positionData[0]), float.Parse(positionData[1]), float.Parse(positionData[2])),
                new Vector3(float.Parse(scaleData[0]), float.Parse(scaleData[1]), float.Parse(scaleData[2])),
                Quaternion.Euler(float.Parse(rotationData[0]), float.Parse(rotationData[1]), float.Parse(rotationData[2])));
        }
    }
}

[Serializable]
public struct CitySpawnable
{
    public GameObject spawnObject;
    public Transform spawnParent;
    public int minAmount, maxAmount;
    [Space(15)] public Vector3 spawnPosMin, spawnPosMax;
}
