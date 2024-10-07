using UnityEngine;
using Cinemachine;

public class CameraDistanceAdjuster : MonoBehaviour
{
    public DungBallMovementController dungBallController;
    public float minDistance = 1.5f;
    public float maxDistance = 3f;
    public float distanceMultiplier = 0.02f;
    public float smoothTime = 1f; // 添加平滑时间参数

    private CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow _3rdFollow;
    private float currentVelocity; // 用于SmoothDamp

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _3rdFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        if (dungBallController == null)
        {
            dungBallController = FindObjectOfType<DungBallMovementController>();
        }
    }

    private void FixedUpdate()
    {
        if (dungBallController != null && _3rdFollow != null)
        {
            float ballSize = dungBallController.CurrentSize;
            float targetDistance = Mathf.Clamp(2-(1-ballSize) * distanceMultiplier, minDistance, maxDistance);
            // Debug.Log("currentDistance:"+targetDistance);
            //Vector3 currentOffset = transposer.m_FollowOffset;
            float currentDistance = _3rdFollow.CameraDistance;
            float currentArmlength = _3rdFollow.VerticalArmLength;

            float newDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref currentVelocity, smoothTime);
            float newArmlength = Mathf.SmoothDamp(currentArmlength, targetDistance/2, ref currentVelocity, smoothTime);
            _3rdFollow.CameraDistance = newDistance;
            _3rdFollow.VerticalArmLength = newArmlength;

        }
    }
}