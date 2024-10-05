using UnityEngine;
using System.Collections.Generic;

public class DungPileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dungPilePrefab;
    [SerializeField] private int maxDungPiles = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float minSpawnSize = 0.5f;
    [SerializeField] private float maxSpawnSize = 2f;

    private List<GameObject> activeDungPiles = new List<GameObject>();
    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && activeDungPiles.Count < maxDungPiles)
        {
            SpawnDungPile();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnDungPile()
    {
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        if (groundObjects.Length == 0)
        {
            Debug.LogWarning("No objects with 'Ground' tag found!");
            return;
        }

        GameObject randomGround = groundObjects[Random.Range(0, groundObjects.Length)];
        Renderer groundRenderer = randomGround.GetComponent<Renderer>();

        if (groundRenderer == null)
        {
            Debug.LogWarning("Selected ground object has no Renderer component!");
            return;
        }

        Vector3 randomPosition = GetRandomPositionOnGround(groundRenderer.bounds);
        float randomSize = Random.Range(minSpawnSize, maxSpawnSize);

        GameObject dungPile = Instantiate(dungPilePrefab, randomPosition, Quaternion.identity);
        DungPile dungPileComponent = dungPile.GetComponent<DungPile>();
        if (dungPileComponent != null)
        {
            dungPileComponent.dungSize = randomSize;
        }

        activeDungPiles.Add(dungPile);
    }

    private Vector3 GetRandomPositionOnGround(Bounds groundBounds)
    {
        float randomX = Random.Range(groundBounds.min.x, groundBounds.max.x);
        float randomZ = Random.Range(groundBounds.min.z, groundBounds.max.z);
        float y = groundBounds.max.y + 0.1f; // Slightly above the ground

        return new Vector3(randomX, y, randomZ);
    }

    public void RemoveDungPile(GameObject dungPile)
    {
        activeDungPiles.Remove(dungPile);
    }
}