using UnityEngine;
using System;
using PaintIn3D;

public class DungBallController : MonoBehaviour
{
    public GameObject shitball;
    public GameObject beetle;
    public ParticleSystem DungRollingPS;
    public ParticleSystem DungPilePickUpPS;

    private DungBallMovementController movementController;
    private CustomCwPaintSphere paintSphere;
    private Renderer shitballRenderer;

    private void Awake()
    {
        movementController = GetComponent<DungBallMovementController>();
        paintSphere = GetComponent<CustomCwPaintSphere>();
        shitballRenderer = shitball.GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        movementController.OnSizeChanged += HandleSizeChanged;
        if (paintSphere == null)
        {
            Debug.LogWarning("CustomCwPaintSphere component not found on DungBall!");
        }
    }

    private void OnDestroy()
    {
        movementController.OnSizeChanged -= HandleSizeChanged;
    }

    private void Update()
    {
        HandlePainting();
    }

    private void HandlePainting()
    {
        bool shouldPaint = movementController.HasDung && Input.GetKey(KeyCode.Space);
        paintSphere.SetPaintingAllowed(shouldPaint);
        if (shouldPaint)
        {
            // EffectManager.Instance.PlayEffect("DungballRolling", shitball.transform.position);
            if (!DungRollingPS.isPlaying)
            {
                DungRollingPS.Play();
            }
            movementController.ConsumeDung(Time.deltaTime);
        }
        else
        {
            if (DungRollingPS.isPlaying)
            {
                DungRollingPS.Stop();
            }
        }
    }

    private void HandleSizeChanged(float newSize)
    {
        if (paintSphere != null)
        {
            paintSphere.Radius = newSize * 0.2f;
        }
    }

    public void PickUpDung(DungPile dungPile)
    {
        if (!DungPilePickUpPS.isPlaying)
        {
            DungPilePickUpPS.Play();
        }
        movementController.IncreaseSize(dungPile.dungSize);
    }

    public void AdjustDungballPosition(float ratio)
    {
        if (shitball != null && beetle != null && shitballRenderer != null)
        {
            var newpos = new Vector3(0, shitball.transform.localPosition.y, shitball.transform.localPosition.z);
            newpos.y *= ratio;
            newpos.z *= ratio;

            shitball.transform.localPosition = newpos;
        }
    }
}