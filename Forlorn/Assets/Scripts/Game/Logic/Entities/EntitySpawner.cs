using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject entityPrefab;

    private List<GameObject> entities = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnInitialEntities());
    }

    IEnumerator SpawnInitialEntities()
    {
        yield return new WaitUntil(() => WorldGeneration.worldLoaded);

        for (int i = 0; i < 50; i++)
        {
            GameObject entity = GetEntity();

            Vector2 position = new Vector3(Random.Range(WorldGeneration.worldBounds.Item1, WorldGeneration.worldBounds.Item2), -0.12f);
            while (PlayerCamera.IsPositionOnScreen(position))
            {
                position = new Vector3(Random.Range(WorldGeneration.worldBounds.Item1, WorldGeneration.worldBounds.Item2), -0.12f);
                yield return new WaitForEndOfFrame();
            }

            entity.transform.position = position;
        }
    }

    private GameObject GetEntity()
    {
        foreach (GameObject entity in entities)
            if (!entity.activeSelf)
            {
                entity.SetActive(true);
                return entity;
            }

        GameObject newEntity = Instantiate(entityPrefab, transform);
        entities.Add(newEntity);
        return newEntity;
    }
}
