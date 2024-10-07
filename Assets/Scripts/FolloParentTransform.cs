using UnityEngine;

public class FollowParentTransform : MonoBehaviour
{
    private Vector3 initialLocalPosition;
    private Vector3 initialLocalScale;
    private Quaternion initialRotation;

    private void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // 保持全局位置和缩放
        transform.position = transform.parent.TransformPoint(initialLocalPosition);
        transform.localScale = Vector3.Scale(initialLocalScale, transform.parent.lossyScale);

        // 保持初始旋转
        transform.rotation = initialRotation;
    }
}