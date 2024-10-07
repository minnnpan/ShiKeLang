using UnityEngine;
using Cinemachine;

public class CameraDistanceAdjuster : MonoBehaviour
{
    public DungBallMovementController dungBallController;
    public float minDistance = 0.05f;
    public float maxDistance = 15f;
    public float distanceMultiplier = 1.05f;
    public float smoothTime = 1f; // 添加平滑时间参数

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;
    private float currentVelocity; // 用于SmoothDamp

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        if (dungBallController == null)
        {
            dungBallController = FindObjectOfType<DungBallMovementController>();
        }
    }

    private void Update()
    {
        if (dungBallController != null && transposer != null)
        {
            float ballSize = dungBallController.CurrentSize;
            float targetDistance = Mathf.Clamp(ballSize * distanceMultiplier, minDistance, maxDistance);
            
            Vector3 currentOffset = transposer.m_FollowOffset;
            float currentDistance = -currentOffset.z;

            float newDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref currentVelocity, smoothTime);
            
            Vector3 newOffset = currentOffset;
            newOffset.z = -newDistance;
            transposer.m_FollowOffset = newOffset;
        }
    }
}