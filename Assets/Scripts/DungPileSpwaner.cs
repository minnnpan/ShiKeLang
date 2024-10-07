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

        float x = Random.Range(3f, 8f);
        float z = Random.Range(3f, 8f);
        float i = Random.Range(0, 1);
        if (i > 0.5f)
        {
            i = -1;
        }
        else
        {
            i = 1;
        }
        Vector3 randomPosition = player.transform.position + Vector3.forward * x * i - Vector3.right * z * i;
        float randomSize = Random.Range(minSpawnSize, maxSpawnSize);

        Vector3 spawnPosition = randomPosition + Vector3.up * spawnHeight;
        GameObject dungPile = Instantiate(dungPilePrefab, spawnPosition, dungPilePrefab.transform.rotation);
        dungPile.transform.localScale = dungPilePrefab.transform.localScale * randomSize;

        // 设置父物体
        dungPile.transform.SetParent(dungParent, true);

        DungPile dungPileComponent = dungPile.GetComponent<DungPile>();
        if (dungPileComponent != null)
        {
            dungPileComponent.dungSize = randomSize;
        }

        // 使用DOTween实现落下动画
        dungPile.transform.DOMoveY(randomPosition.y, fallDuration).SetEase(Ease.OutBounce)
            .OnComplete(() => {
                // 触发特效，保持预制体的本地位置和旋转
                GameObject effect = EffectManager.Instance.PlayEffect("DungBallFalling",
                    new Vector3(randomPosition.x, 2, randomPosition.z));
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
}