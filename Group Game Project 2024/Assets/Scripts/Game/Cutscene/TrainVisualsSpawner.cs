using UnityEngine;

public class TrainVisualsSpawner : MonoBehaviour
{
    [SerializeField] private Transform carriageHolder;
    [SerializeField] private Transform lightsHolder;
    [SerializeField] private Transform billboardHolder;
    [Space(15), SerializeField] private GameObject carriagePrefab;
    [SerializeField] private GameObject lightPrefab;
    [Space(15), SerializeField] private float carriageSpawnDistance;

    private float lightSpawnCooldown;
    private Camera mainCam;
    private GameObject lastSpawnedCarriage;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        SpawnCarriage();
        SpawnLight();
    }

    private void SpawnCarriage()
    {
        if (lastSpawnedCarriage == null || (carriagePrefab.transform.position - lastSpawnedCarriage.transform.position).magnitude > carriageSpawnDistance * carriageSpawnDistance)
        {
            GameObject newCarriage = Instantiate(carriagePrefab, carriageHolder);
            newCarriage.SetActive(true);
            Destroy(newCarriage, 30f);

            lastSpawnedCarriage = newCarriage;
        }
    }

    private void SpawnLight()
    {
        lightSpawnCooldown -= Time.deltaTime;
        if (lightSpawnCooldown <= 0)
        {
            GameObject newLight = Instantiate(lightPrefab, lightsHolder);
            newLight.SetActive(true);
            Destroy(newLight, 10f);

            lightSpawnCooldown = Random.Range(5f, 8f);
        }
    }
}
