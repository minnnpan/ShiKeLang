using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class DungBallMovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 30f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float airControlFactor = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Size")]
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 3f;
    [SerializeField] private float shrinkRate = 0.1f;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSize = 1f;
    private Vector3 moveDirection;

    public event Action<float> OnSizeChanged;
    public event Action<float> OnDungPickup;
    public event Action<float> OnDungDrop;

    public float CurrentSize => currentSize;
    public bool HasDung => currentSize > 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        UpdateSize();
    }

    private void Update()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
        HandleRotation();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }

    private void HandleMovement()
    {
        Vector3 targetVelocity = moveDirection * maxSpeed;
        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0f;

        float controlFactor = isGrounded ? 1f : airControlFactor;
        rb.AddForce(velocityChange * moveForce * controlFactor, ForceMode.Acceleration);

        if (isGrounded)
        {
            rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void HandleRotation()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    public void ConsumeDung(float amount)
    {
        if (currentSize > minSize)
        {
            currentSize = Mathf.Max(minSize, currentSize - amount);
            UpdateSize();
            if (currentSize <= 0.1f)
            {
                OnDungDrop?.Invoke(currentSize);
            }
        }
    }

    private void UpdateSize()
    {
        transform.localScale = Vector3.one * currentSize;
        rb.mass = currentSize;
        OnSizeChanged?.Invoke(currentSize);
    }

    public void IncreaseSize(float amount)
    {
        float previousSize = currentSize;
        currentSize = Mathf.Min(maxSize, currentSize + amount);
        UpdateSize();
        OnDungPickup?.Invoke(currentSize - previousSize);
    }

    public void InitializeSize(float size)
    {
        currentSize = size;
        UpdateSize();
    }
}