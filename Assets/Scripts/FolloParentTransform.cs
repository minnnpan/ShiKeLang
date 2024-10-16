using UnityEngine;

public class FollowParentTransform : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 initialScale = Vector3.one;

    private Vector3 initialLocalPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        if (targetTransform == null)
        {
            targetTransform = transform.parent;
        }

        //initialLocalPosition = transform.localPosition;
        //initialRotation = transform.rotation;
        transform.localScale = initialScale;
    }

    private void LateUpdate()
    {
        if (targetTransform != null)
        {
            // 保持全局位置
            //transform.position = targetTransform.TransformPoint(initialLocalPosition);

            // 跟随目标的缩放变化
            transform.localScale = Vector3.Scale(initialScale, targetTransform.lossyScale);

            // 保持初始旋转
            //transform.rotation = initialRotation;
        }
    }
}
