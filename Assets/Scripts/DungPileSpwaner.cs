using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class DungPileSpawner : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private GameObject dungPilePrefab;
    [SerializeField] private int maxDungPiles = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float minSpawnSize = 0.5f;
    [SerializeField] private float maxSpawnSize = 2f;
    [SerializeField] private float spawnHeight = 5f;
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private Transform dungParent;

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
            spawnInterval = Random.Range(2, 5);
        }
    }

    private void SpawnDungPile()
    {
        float randomSize = Random.Range(minSpawnSize, maxSpawnSize);
        Vector3 spawnPosition;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            float x = Random.Range(3f, 8f) * (Random.value > 0.5f ? 1 : -1);
            float z = Random.Range(3f, 8f) * (Random.value > 0.5f ? 1 : -1);
            spawnPosition = player.transform.position + Vector3.forward * x - Vector3.right * z;
            attempts++;
        } while (!IsValidSpawnPosition(spawnPosition, randomSize) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Failed to find a valid spawn position for DungPile");
            return;
        }

        Vector3 finalSpawnPosition = spawnPosition + Vector3.up * spawnHeight;
        GameObject dungPile = Instantiate(dungPilePrefab, finalSpawnPosition, dungPilePrefab.transform.rotation);
        dungPile.transform.localScale = dungPilePrefab.transform.localScale * randomSize;

        dungPile.transform.SetParent(dungParent, true);

        DungPile dungPileComponent = dungPile.GetComponent<DungPile>();
        if (dungPileComponent != null)
        {
            dungPileComponent.dungSize = randomSize;
        }

        dungPile.transform.DOMoveY(spawnPosition.y, fallDuration).SetEase(Ease.OutBounce)
            .OnComplete(() => {
                GameObject effect = EffectManager.Instance.PlayEffect("DungBallFalling",
                    new Vector3(spawnPosition.x, 2, spawnPosition.z));
            });

        activeDungPiles.Add(dungPile);
    }

    private Vector3 GetRandomPositionOnGround(Bounds groundBounds)
    {
        float randomX = Random.Range(groundBounds.min.x, groundBounds.max.x);
        float randomZ = Random.Range(groundBounds.min.z, groundBounds.max.z);
        float y = groundBounds.max.y;

        return new Vector3(randomX, y, randomZ);
    }

    public void RemoveDungPile(GameObject dungPile)
    {
        activeDungPiles.Remove(dungPile);
    }

    private bool IsValidSpawnPosition(Vector3 position, float dungSize)
    {
        // 射线检测
        RaycastHit hit;
        if (!Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            return false;
        }

        // 重叠检测
        Collider[] colliders = Physics.OverlapSphere(hit.point + Vector3.up * dungSize * 0.5f, dungSize * 0.5f);
        return colliders.Length == 0;
    }
}
