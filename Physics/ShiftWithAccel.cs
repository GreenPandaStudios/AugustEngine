namespace AugustEngine.Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    public class ShiftWithAccel : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] [Range(0, 1f)] float scale;
        [SerializeField] [Range(0, 1f)] float acceleration;
        [SerializeField] float clamp;

        private void OnEnable()
        {
            //make sure fixedupdate is instantiated
            if (FixedUpdateEvent.Instance) { }
            FixedUpdateEvent.OnFixedUpdate += Shift;

        }
        private void OnDisable()
        {
            FixedUpdateEvent.OnFixedUpdate -= Shift;
        }

        public void AddForce(Vector3 force)
        {
            lastVelocity -= force;
        }
        private Vector3 lastVelocity = Vector3.zero;


        private void Shift()
        {
            var _accel = (rb.velocity - lastVelocity) * scale;
            if (_accel.magnitude > clamp) _accel = _accel.normalized * clamp;

            if (_accel.magnitude * scale > clamp)
            {
                _accel = (_accel.normalized * clamp) / scale;
            }

            _accel = Vector3.Project(_accel, transform.parent.up);

            transform.position = Vector3.Lerp(transform.position, transform.parent.position + _accel, acceleration);

            lastVelocity = Vector3.Lerp(lastVelocity, rb.velocity, .25f);

        }
    }
}
