using UnityEngine;
using System;
using DG.Tweening;
using PaintIn3D;

public class DungBallController : MonoBehaviour
{
    [SerializeField] private GameObject beetlePrefab;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private DungTrailManager trailManager;
    [SerializeField] private float beetleGroundOffset = 0.1f;
    [SerializeField] private float minDistanceToBall = 0.5f;
    [SerializeField] private float maxDistanceToBall = 1.5f;

    private float currentDungSize = 1f;
    private GameObject beetle;
    private Vector3 beetleVelocity;
    private DungBallMovementController movementController;
    private Rigidbody beetleRigidbody;
    private CwPaintSphere paintSphere;

    public bool HasDung => currentDungSize > 0.1f;

    public event Action<float> OnDungPickup;
    public event Action<float> OnDungDrop;

    private void Awake()
    {
        movementController = GetComponent<DungBallMovementController>();
        paintSphere = GetComponent<CwPaintSphere>();
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        InitializeBeetle();
        UpdateDungBallSize();
        movementController.OnSizeChanged += HandleSizeChanged;
        if (paintSphere == null)
        {
            Debug.LogWarning("CwPaintSphere component not found on DungBall!");
        }
    }

    private void InitializeBeetle()
    {
        beetle = Instantiate(beetlePrefab, transform.position, Quaternion.identity);
        beetleRigidbody = beetle.GetComponent<Rigidbody>();
        if (beetleRigidbody == null)
        {
            beetleRigidbody = beetle.AddComponent<Rigidbody>();
        }
        beetleRigidbody.useGravity = true;
        beetleRigidbody.drag = 0f;
        beetleRigidbody.angularDrag = 0f;
        beetleRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void Update()
    {
        UpdateBeetlePosition();

        if (HasDung && Input.GetKey(KeyCode.Space))
        {
            movementController.ShrinkDungBall();
        }
    }

    private void UpdateBeetlePosition()
    {
        if (beetle != null)
        {
            Vector3 directionToBeetle = (beetle.transform.position - transform.position);
            float distanceToBall = directionToBeetle.magnitude;

            // 根据粪球大小调整最小和最大距离
            float adjustedMinDistance = minDistanceToBall + currentDungSize * 0.5f;
            float adjustedMaxDistance = maxDistanceToBall + currentDungSize * 0.5f;

            Vector3 targetPosition;
            if (distanceToBall > adjustedMaxDistance)
            {
                targetPosition = transform.position + directionToBeetle.normalized * adjustedMaxDistance;
            }
            else if (distanceToBall < adjustedMinDistance)
            {
                targetPosition = transform.position + directionToBeetle.normalized * adjustedMinDistance;
            }
            else
            {
                return; // 如果在合适的范围内，不需要调整位置
            }

            // 只更新 x 和 z 坐标，让 y 坐标由物理系统处理
            Vector3 newPosition = Vector3.Lerp(beetle.transform.position, targetPosition, Time.deltaTime / smoothTime);
            beetle.transform.position = new Vector3(newPosition.x, beetle.transform.position.y, newPosition.z);

            // 使屎壳郎面向粪球
            Vector3 lookDirection = transform.position - beetle.transform.position;
            lookDirection.y = 0;
            beetle.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    private void HandleSizeChanged(float newSize)
    {
        currentDungSize = newSize;
        UpdateDungBallSize();

        if (currentDungSize <= 0.1f)
        {
            DropDung();
        }
    }

    private void UpdateDungBallSize()
    {
        transform.localScale = Vector3.one * currentDungSize;
        if (paintSphere != null)
        {
            paintSphere.Radius = currentDungSize * 0.5f; // 你可以调整这个乘数来获得合适的效果
        }
    }

    private void DropDung()
    {
        OnDungDrop?.Invoke(currentDungSize);
        currentDungSize = 0;
        UpdateDungBallSize();
    }

    public void PickUpDung(DungPile dungPile)
    {
        movementController.IncreaseSize(dungPile.dungSize);
        OnDungPickup?.Invoke(dungPile.dungSize);
    }
}