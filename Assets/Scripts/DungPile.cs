using UnityEngine;

public class DungPile : MonoBehaviour
{
    public float dungSize = 1f;
    private DungPileSpawner spawner;

    private void Start()
    {
        spawner = FindObjectOfType<DungPileSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DungBeetle dungBeetle = other.GetComponent<DungBeetle>();
            if (dungBeetle != null)
            {
                Debug.Log("拾取大便");
                dungBeetle.PickUpDung(this);
                if (spawner != null)
                {
                    spawner.RemoveDungPile(gameObject);
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("dungBeetle is null");
            }
        }
    }
}