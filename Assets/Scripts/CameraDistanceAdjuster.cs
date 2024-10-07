using UnityEngine;
using Cinemachine;

public class CameraDistanceAdjuster : MonoBehaviour
{
    public DungBallMovementController dungBallController;
    public float minDistance = 1f;
    public float maxDistance = 15f;
    public float distanceMultiplier = 1.5f;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;

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
            float newDistance = Mathf.Clamp(ballSize * distanceMultiplier, minDistance, maxDistance);
            
            Vector3 newOffset = transposer.m_FollowOffset;
            newOffset.z = -newDistance;
            transposer.m_FollowOffset = newOffset;
        }
    }
}