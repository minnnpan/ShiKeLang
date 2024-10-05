using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f; // 添加重力
    [SerializeField] private float groundedTolerance = 0.1f; // 地面检测容差

    private CharacterController controller;
    private Vector3 movement;
    private Vector3 verticalVelocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 检查是否在地面上
        isGrounded = controller.isGrounded;

        // 应用重力
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // 小的向下力，确保角色紧贴地面
        }
        verticalVelocity.y += gravity * Time.deltaTime;

        // 获取输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 计算移动向量（只在 X-Z 平面上）
        movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 移动角色
        if (movement.magnitude >= 0.1f)
        {
            // 确保移动只在水平面上
            Vector3 horizontalMovement = movement * moveSpeed * Time.deltaTime;
            controller.Move(horizontalMovement + verticalVelocity * Time.deltaTime);

            // 角色朝向移动方向
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 即使不移动，也应用重力
            controller.Move(verticalVelocity * Time.deltaTime);
        }

        // 额外的地面检测
        if (Physics.Raycast(transform.position, Vector3.down, groundedTolerance))
        {
            isGrounded = true;
        }
    }
}