using UnityEngine;
using DG.Tweening;

public class DungPile : MonoBehaviour
{
    public float dungSize = 1f;
    private DungPileSpawner spawner;
    private bool canBePickedUp = false;

    private void Start()
    {
        spawner = FindObjectOfType<DungPileSpawner>();
        // 等待落地动画完成后才能被拾取
        DOVirtual.DelayedCall(1f, () => canBePickedUp = true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBePickedUp && other.CompareTag("Player"))
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