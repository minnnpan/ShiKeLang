using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -5);
    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.LookAt(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}