using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -5f);

    [Header("Camera Angle")]
    [Range(0f, 90f)]
    [SerializeField] private float angle = 45f;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float currentZoom = 10f;

    private void LateUpdate()
    {
        // 更新缩放
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 计算目标位置
        Vector3 targetPosition = target.position + Quaternion.Euler(angle, 0f, 0f) * new Vector3(0f, 0f, -currentZoom);

        // 平滑移动摄像机
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 让摄像机看向目标
        transform.LookAt(target);
    }
}