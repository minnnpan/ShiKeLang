using UnityEngine;
using System;
using UnityEngine.Serialization;

public class DungBallMovementController : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 3f;
    [SerializeField] private GameObject shitBall;
    [SerializeField] private float consumeSpeed = 0.1f; // 新增: 消耗速度
    private Vector3 initialDungLocalScale;
    private Vector3 initialDungLocalPosition;

    private float currentSize = 1f;

    public event Action<float> OnSizeChanged;
    public event Action<float> OnDungPickup;
    public event Action<float> OnDungDrop;

    public float CurrentSize => currentSize;
    public bool HasDung => currentSize > minSize;

    private void Start()
    {
        UpdateSize(1f);
        initialDungLocalScale = shitBall.transform.localScale;
        initialDungLocalPosition = shitBall.transform.localPosition;
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
            if (currentSize <= minSize)
            {
                OnDungDrop?.Invoke(consumeAmount);
            }
        }
    }

    private void UpdateSize(float ratio)
    {
        shitBall.transform.localScale *= ratio;
        OnSizeChanged?.Invoke(currentSize);
        GetComponent<DungBallController>().AdjustDungballPosition(ratio);
    }

    public void IncreaseSize(float amount)
    {
        float previousSize = currentSize;
        currentSize = Mathf.Min(maxSize, currentSize + amount);
        float ratio = currentSize / previousSize;
        UpdateSize(ratio);
        OnDungPickup?.Invoke(amount);
    }

    public void InitializeSize()
    {
        shitBall.transform.localPosition = initialDungLocalPosition;
        shitBall.transform.localScale = initialDungLocalScale;

        currentSize = 1f;
    }
}