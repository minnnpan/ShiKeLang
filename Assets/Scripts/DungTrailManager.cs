using UnityEngine;
using System.Collections.Generic;

public class DungTrailManager : MonoBehaviour
{
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private float decalLifetime = 10f;
    [SerializeField] private int maxDecals = 100;

    private Queue<GameObject> activeDecals = new Queue<GameObject>();

    public void SpawnDecal(Vector3 position, float size, Quaternion rotation)
    {
        if (activeDecals.Count >= maxDecals)
        {
            GameObject oldestDecal = activeDecals.Dequeue();
            Destroy(oldestDecal);
        }

        GameObject decal = Instantiate(decalPrefab, position, rotation);
        decal.transform.localScale = Vector3.one * size;
        activeDecals.Enqueue(decal);

    }
}