using UnityEngine;

public class DungPile : MonoBehaviour
{
    public float dungSize = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DungBeetle dungBeetle = other.GetComponent<DungBeetle>();
            if (dungBeetle != null)
            {
                Debug.Log("拾取大便");
                dungBeetle.PickUpDung(this);
                Destroy(gameObject);
            }else{
                Debug.Log("dungBeetle is null");
            }
        }
    }
}