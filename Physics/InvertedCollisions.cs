namespace AugustEngine.Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Calculates phyics collisions as if they occur inside a collider
    /// Requires the collider contianing the object to be a trigger,
    /// and that the contained object has this script attached
    /// Also requires that any inverted collider trigger be tagged with
    /// InvertedCollider
    /// </summary>
    public class InvertedCollisions : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("InvertedCollider"))
            {
                //we have exited an inverted collider, move to the edge
                rb.transform.position = other.ClosestPoint(rb.transform.position);

                //get the physics material
                //is there a rigidbody?
                Physics.Raycast(rb.position, rb.velocity, out var _hit, 1f, other.gameObject.layer, QueryTriggerInteraction.Collide);
                rb.velocity = Vector3.Reflect(rb.velocity * other.sharedMaterial.bounciness, _hit.normal);
                if (other.attachedRigidbody)
                {
                    rb.velocity += other.attachedRigidbody.velocity * Mathf.Abs(Vector3.Dot(rb.velocity, _hit.normal)) * other.sharedMaterial.bounciness
                         + other.attachedRigidbody.velocity * (1f - Mathf.Abs(Vector3.Dot(rb.velocity, _hit.normal))) * other.sharedMaterial.dynamicFriction;
                }
            }
        }
    }
}
