using UnityEngine;

public class DungPile : MonoBehaviour
{
    public float dungSize = 1.5f;
    private DungPileSpawner spawner;

    private void Start()
    {
        spawner = FindObjectOfType<DungPileSpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DungBallController dungBallController = other.GetComponent<DungBallController>();
            if (dungBallController != null)
            {
                Debug.Log("拾取大便");
                dungBallController.PickUpDung(this);
                if (spawner != null)
                {
                    spawner.RemoveDungPile(gameObject);
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("dungBallController is null");
            }
        }
    }
}