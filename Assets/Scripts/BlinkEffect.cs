using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    public float blinkDuration = 0.1f; // 每次闪烁的持续时间
    public int blinkCount = 5; // 闪烁次数

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        StartCoroutine(Blink());
    }

    System.Collections.IEnumerator Blink()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            objectRenderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration);
            objectRenderer.enabled = true;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}