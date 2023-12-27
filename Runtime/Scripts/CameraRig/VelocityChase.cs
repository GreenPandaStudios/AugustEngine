namespace AugustEngine.CameraRig
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class VelocityChase : MonoBehaviour
    {

        [SerializeField] float orbitDistance;
        [SerializeField] Rigidbody basedOnVelocity;
        [SerializeField] [Range(0, 1f)] float moveAcceleration;
        [SerializeField] [Range(0, 1f)] float heightBias;
        [SerializeField] float minimumMovementMagniude;


        private Vector3 targetPos = Vector3.zero;
        private void OnEnable()
        {
            targetPos = transform.localPosition;

            LowLevel.FixedUpdateEvent.Initialize();
            LowLevel.FixedUpdateEvent.OnFixedUpdate += MoveRig;
        }
        private void OnDisable()
        {
            LowLevel.FixedUpdateEvent.OnFixedUpdate -= MoveRig;
        }
        // Update is called once per frame
        void MoveRig()
        {
            //check if we are above the velocity magnitude to warrent change
            if (basedOnVelocity.velocity.magnitude > minimumMovementMagniude)
            {
                //project the opposite of velocity to the orbit sphere
                targetPos = (-basedOnVelocity.velocity.normalized + Vector3.up * heightBias) * orbitDistance;
            }
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, moveAcceleration);
        }
    }
}
