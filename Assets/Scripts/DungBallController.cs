using UnityEngine;
using System;
using DG.Tweening;
using PaintIn3D;

public class DungBallController : MonoBehaviour
{
    [SerializeField] private GameObject beetlePrefab;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float beetleGroundOffset = 0.1f;
    [SerializeField] private float minDistanceToBall = 0.5f;
    [SerializeField] private float maxDistanceToBall = 1.5f;

    private GameObject beetle;
    private Vector3 beetleVelocity;
    private DungBallMovementController movementController;
    private Rigidbody beetleRigidbody;
    private CustomCwPaintSphere paintSphere;

    private void Awake()
    {
        movementController = GetComponent<DungBallMovementController>();
        paintSphere = GetComponent<CustomCwPaintSphere>();
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        InitializeBeetle();
        movementController.OnSizeChanged += HandleSizeChanged;
        if (paintSphere == null)
        {
            Debug.LogWarning("CustomCwPaintSphere component not found on DungBall!");
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
        HandlePainting();
    }

    private void HandlePainting()
    {
        bool shouldPaint = movementController.HasDung && Input.GetKey(KeyCode.Space);
        paintSphere.SetPaintingAllowed(shouldPaint);
        if (shouldPaint)
        {
            movementController.ConsumeDung(0.1f * Time.deltaTime);
        }
    }

    private void UpdateBeetlePosition()
    {
        if (beetle != null)
        {
            Vector3 directionToBeetle = (beetle.transform.position - transform.position);
            float distanceToBall = directionToBeetle.magnitude;

            float adjustedMinDistance = minDistanceToBall + movementController.CurrentSize * 0.5f;
            float adjustedMaxDistance = maxDistanceToBall + movementController.CurrentSize * 0.5f;

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
                return;
            }

            Vector3 newPosition = Vector3.Lerp(beetle.transform.position, targetPosition, Time.deltaTime / smoothTime);
            beetle.transform.position = new Vector3(newPosition.x, beetle.transform.position.y, newPosition.z);

            Vector3 lookDirection = transform.position - beetle.transform.position;
            lookDirection.y = 0;
            beetle.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    private void HandleSizeChanged(float newSize)
    {
        if (paintSphere != null)
        {
            paintSphere.Radius = newSize * 0.5f;
        }
    }

    public void PickUpDung(DungPile dungPile)
    {
        movementController.IncreaseSize(dungPile.dungSize);
    }
}