namespace AugustEngine.Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    using AugustEngine.Vectors;
    public class TargetVelocitySetter : MonoBehaviour
    {
        [SerializeField] bool ignoreY = true;
        [Tooltip("What rigidbody to effect")]
        [SerializeField] Rigidbody rb;
        [SerializeField]
        [Tooltip("What is the accerleration in m/s/s?")]
        public float Acceleration;

        [SerializeField]
        [Tooltip("What is the target Velocity in m/s?")]
        public Vector3 TargetVelocity;


        public Rigidbody RB { get => rb; }
        private void OnEnable()
        {
            FixedUpdateEvent.Initialize();

            FixedUpdateEvent.OnFixedUpdate += MoveTowardsTarget;
        }

        private void OnDisable()
        {
            FixedUpdateEvent.OnFixedUpdate -= MoveTowardsTarget;
        }

        private void MoveTowardsTarget()
        {

            if (ignoreY)
            {
                TargetVelocity = new Vector3(TargetVelocity.x,
                    rb.velocity.y, TargetVelocity.z);
            }

            rb.AddForce(
                rb.velocity.To(TargetVelocity) * Acceleration * Time.fixedDeltaTime,
                ForceMode.VelocityChange
                );
        }


    }
}


