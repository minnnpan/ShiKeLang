using UnityEngine;
using System.Collections.Generic;
using CW.Common;
using PaintCore;

namespace PaintIn3D
{
    /// <summary>
    /// This component will perform a raycast based on collision events instead of mouse or finger clicks, allowing you to paint and draw continuous lines on the scene.
    /// </summary>
    [HelpURL(CwCommon.HelpUrlPrefix + "CwHitCl")]
    [AddComponentMenu(PaintCore.CwCommon.ComponentHitMenuPrefix + "Hit Collision Weidi")]
    public class CwHitCl : MonoBehaviour
    {
        public LayerMask collisionLayers = -1;  // Set layers to detect collisions with
        public float pressureMultiplier = 1.0f;  // Multiplier for pressure value
        public float raycastDistance = 0.0001f;  // Raycast distance for accurate hit detection on the surface
        public float drawingInterval = 0.05f; // Interval between drawing new points on the surface
        public float positionThreshold = 0.01f; // Minimum distance between points to draw new ones

        [SerializeField]
        private CwPointConnector connector = new CwPointConnector();  // Handles connection of paint points
        [SerializeField]
        private CwHitCache hitCache = new CwHitCache();  // Stores cached hit components

        private Dictionary<GameObject, Vector3> lastPositions = new Dictionary<GameObject, Vector3>();  // To track the last position of each colliding object
        private Dictionary<GameObject, float> lastDrawTime = new Dictionary<GameObject, float>();  // To track the last draw time of each colliding object

        // Accessor for the hit cache
        public CwHitCache HitCache
        {
            get { return hitCache; }
        }

        // Clears the hit cache
        public void ClearHitCache()
        {
            connector.ClearHitCache();
        }

        // This is triggered when a collision occurs
        protected virtual void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }

        // This is triggered when a collision persists, allowing continuous drawing while the object is in contact
        protected virtual void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        // Handles collision logic and applies painting at the point of collision
        private void HandleCollision(Collision collision)
        {
            // Ignore impulse logic, proceed with drawing for every collision

            // Handle each contact point in the collision
            foreach (var contact in collision.contacts)
            {
                if (CwHelper.IndexInMask(contact.otherCollider.gameObject.layer, collisionLayers))
                {
                    var finalPressure = pressureMultiplier;
                    var position = contact.point;
                    var rotation = Quaternion.LookRotation(-contact.normal, Vector3.up);  // World up vector

                    // Perform a raycast to improve the accuracy of hit detection if required
                    if (raycastDistance > 0.0f)
                    {
                        var ray = new Ray(contact.point + contact.normal * raycastDistance, -contact.normal);
                        if (contact.otherCollider.Raycast(ray, out RaycastHit hit, raycastDistance * 2.0f))
                        {
                            position = hit.point;
                        }
                    }

                    // Only draw if enough time has passed or the object has moved sufficiently
                    if (ShouldDrawNewPoint(contact.otherCollider.gameObject, position))
                    {
                        // Apply paint at the collision point and connect to the previous point
                        PaintContinuousLine(position, rotation, finalPressure, contact.otherCollider.gameObject);

                        // Update the last draw time and position
                        lastDrawTime[contact.otherCollider.gameObject] = Time.time;
                        lastPositions[contact.otherCollider.gameObject] = position;
                    }
                }
            }
        }

        // Determines whether a new point should be drawn based on position and time
        private bool ShouldDrawNewPoint(GameObject owner, Vector3 newPosition)
        {
            if (!lastDrawTime.ContainsKey(owner) || Time.time - lastDrawTime[owner] > drawingInterval)
            {
                // Check the distance between the last position and the new position
                if (!lastPositions.ContainsKey(owner) || Vector3.Distance(lastPositions[owner], newPosition) > positionThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        // The method that actually paints at the given position and connects it to the previous point for a continuous line
        private void PaintContinuousLine(Vector3 position, Quaternion rotation, float pressure, GameObject owner)
        {
            // Submit the new point and connect it to the last position
            connector.SubmitPoint(gameObject, false, 0, pressure, position, rotation, owner);
        }

        // Clears hit connections and the last position dictionary
        public void ResetConnections()
        {
            connector.ResetConnections();
            lastPositions.Clear();
            lastDrawTime.Clear();
        }

        // Updates the connector for connecting hits smoothly
        protected virtual void Update()
        {
            connector.Update();
        }
    }
}


