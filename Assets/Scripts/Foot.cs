using UnityEngine;

public class Foot: MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Beetle"))
        {
            GameManager.Instance.EndGame(false);
        }
    }
}
