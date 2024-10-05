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

    // decal system
    [SerializeField] private DungTrailManager trailManager;
    [SerializeField] private float decalSpawnInterval = 0.5f;
    private float lastDecalSpawnTime;

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        currentMoveSpeed = baseMoveSpeed;
        fixedYPosition = 0.1f; // 设置一个接近地面的固定高度
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

            // 通知 GameManager 玩家已经丢弃了粪球
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnPlayerDroppedDung();
            }
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

            if (HasDung && Input.GetKey(KeyCode.Space))
            {
                ShrinkDungBall();
                if (Time.time - lastDecalSpawnTime > decalSpawnInterval)
                {
                    SpawnDungDecal();
                    lastDecalSpawnTime = Time.time;
                }
            }
        }
    }

    private void SpawnDungDecal()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.5f; // 提高射线起点
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 1f; // 减小射线距离

        RaycastHit hit;
        if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance))
        {
            Vector3 spawnPosition = hit.point + hit.normal * 0.001f; // 减小偏移量

            // 计算一个保持贴花水平的旋转
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            if (forward == Vector3.zero)
            {
                forward = -Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
            }
            Quaternion horizontalRotation = Quaternion.LookRotation(forward, Vector3.up);

            // 应用90度的X轴旋转来匹配预制体的初始旋转
            Quaternion finalRotation = horizontalRotation * Quaternion.Euler(90, 0, 0);

            trailManager.SpawnDecal(spawnPosition, currentDungSize * 0.5f, finalRotation);
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

    private void ShrinkDungBall()
    {
        float shrinkRate = 0.05f * Time.deltaTime; // 减小收缩速率
        currentDungSize = Mathf.Max(0, currentDungSize - shrinkRate);
        UpdateDungBallSize();
        UpdateMoveSpeed();

        if (currentDungSize <= 0.1f) // 当粪球变得非常时才完全消失
        {
            DropDung();
        }
    }
}