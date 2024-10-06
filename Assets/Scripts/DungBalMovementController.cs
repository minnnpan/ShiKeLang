using UnityEngine;
using System;

public class DungBallMovementController : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 3f;
    [SerializeField] private GameObject shitball;
    [SerializeField] private float consumeSpeed = 0.1f; // 新增: 消耗速度

    private float currentSize = 1f;

    public event Action<float> OnSizeChanged;
    public event Action<float> OnDungPickup;
    public event Action<float> OnDungDrop;

    public float CurrentSize => currentSize;
    public bool HasDung => currentSize > 0.1f;

    private void Start()
    {
        UpdateSize(1f);
    }

    public void ConsumeDung(float deltaTime)
    {
        if (currentSize > minSize)
        {
            float consumeAmount = consumeSpeed * deltaTime;
            float newSize = Mathf.Max(minSize, currentSize - consumeAmount);
            float ratio = newSize / currentSize;
            currentSize = newSize;
            UpdateSize(ratio);
            if (currentSize <= 0.1f)
            {
                OnDungDrop?.Invoke(currentSize);
            }
        }
    }

    private void UpdateSize(float ratio)
    {
        shitball.transform.localScale *= ratio;
        OnSizeChanged?.Invoke(currentSize);
        GetComponent<DungBallController>().AdjustDungballPosition(ratio);
    }

    public void IncreaseSize(float multiplier)
    {
        float previousSize = currentSize;
        currentSize = Mathf.Min(maxSize, currentSize * multiplier);
        float ratio = currentSize / previousSize;
        UpdateSize(ratio);
        OnDungPickup?.Invoke(currentSize - previousSize);
    }
}