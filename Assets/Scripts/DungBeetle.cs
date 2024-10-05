using UnityEngine;
using System;
using DG.Tweening;

public class DungBeetle : MonoBehaviour
{
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float dungBallOffset = 1f;
    [SerializeField] private GameObject dungBallPrefab;
    [SerializeField] private float smoothTime = 0.1f;

    private float currentMoveSpeed;
    private GameObject currentDungBall;
    private float currentDungSize = 0f;
    private Vector3 dungBallVelocity;
    private float fixedYPosition;

    public bool HasDung => currentDungSize > 0;

    public event Action<float> OnDungPickup;
    public event Action<float> OnDungDrop;

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        currentMoveSpeed = baseMoveSpeed;
        fixedYPosition = transform.position.y;
    }

    public void PickUpDung(DungPile dungPile)
    {
        float newSize = currentDungSize + dungPile.dungSize;
        
        if (currentDungBall == null)
        {
            currentDungBall = Instantiate(dungBallPrefab, CalculateDungBallPosition(), Quaternion.identity);
            currentDungBall.transform.SetParent(transform);
        }

        currentDungSize = newSize;
        UpdateDungBallSize();
        UpdateMoveSpeed();

        OnDungPickup?.Invoke(dungPile.dungSize);
    }

    public void DropDung()
    {
        if (HasDung)
        {
            float droppedSize = currentDungSize;
            currentDungSize = 0f;

            Destroy(currentDungBall);
            currentDungBall = null;

            currentMoveSpeed = baseMoveSpeed;

            OnDungDrop?.Invoke(droppedSize);
        }
    }

    private void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 newPosition = transform.position + movement * currentMoveSpeed * Time.deltaTime;
        newPosition.y = fixedYPosition;
        transform.position = newPosition;

        if (movement != Vector3.zero)
        {
            transform.forward = movement;
            UpdateDungBallPosition();
            SimulateDungBallRoll(movement);
        }

        if (Input.GetKeyDown(KeyCode.Space) && HasDung)
        {
            DropDung();
        }
    }

    private void UpdateDungBallSize()
    {
        if (currentDungBall != null)
        {
            currentDungBall.transform.localScale = Vector3.one * currentDungSize;
        }
    }

    private void UpdateDungBallPosition()
    {
        if (currentDungBall != null)
        {
            Vector3 targetPosition = CalculateDungBallPosition();
            targetPosition.y = fixedYPosition + currentDungSize * 0.5f;
            currentDungBall.transform.position = Vector3.SmoothDamp(currentDungBall.transform.position, targetPosition, ref dungBallVelocity, smoothTime);
        }
    }

    private Vector3 CalculateDungBallPosition()
    {
        return transform.position + transform.forward * (dungBallOffset + currentDungSize * 0.5f);
    }

    private void UpdateMoveSpeed()
    {
        float speedReductionFactor = 1f - (currentDungSize * 0.1f);
        speedReductionFactor = Mathf.Clamp(speedReductionFactor, 0.1f, 1f);
        currentMoveSpeed = baseMoveSpeed * speedReductionFactor;
    }

    private void SimulateDungBallRoll(Vector3 movement)
    {
        if (currentDungBall != null && movement.magnitude > 0.001f)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, movement).normalized;
            float rotationAmount = (movement.magnitude / (currentDungSize * Mathf.PI)) * 360f;
            currentDungBall.transform.DORotate(rotationAxis * rotationAmount, Time.deltaTime, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear);
        }
    }
}