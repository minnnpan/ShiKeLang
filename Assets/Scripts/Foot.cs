using UnityEngine;

public class Foot: MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Beetle"))
        {
            EffectManager.Instance.PlayEffect("Dungexploded", collision.transform.position);
            GameManager.Instance.EndGame(false);
        }
    }
}
