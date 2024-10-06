using UnityEngine;
using CW.Common;
using PaintIn3D;

public class CustomCwPaintSphere : CwPaintSphere
{
    private bool isPaintingAllowed = false;

    public void SetPaintingAllowed(bool allowed)
    {
        isPaintingAllowed = allowed;
        enabled = allowed;
    }

    public override void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
    {
        if (isPaintingAllowed)
        {
            base.HandleHitPoint(preview, priority, pressure, seed, position, rotation);
        }
    }
}