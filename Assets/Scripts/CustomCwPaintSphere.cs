using UnityEngine;
using CW.Common;
using PaintIn3D;

public class CustomCwPaintSphere : CwPaintSphere
{
    private DungBallController dungBallController;

    private void Start()
    {
        dungBallController = GetComponent<DungBallController>();
    }

    public override void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
    {
        if (dungBallController != null && dungBallController.IsPaintingAllowed)
        {
            base.HandleHitPoint(preview, priority, pressure, seed, position, rotation);
        }
    }
}