namespace AugustEngine.Physics.Gravity
{
    using UnityEngine;
    using AugustEngine.LowLevel;
    using System;
    using AugustEngine.Vectors;
    public class GroundDetector : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Where to start the raycast from")]
        Transform startCast;
        [SerializeField]
        [Tooltip("Where to end the raycast")]
        Transform endCast;
        [SerializeField]
        [Tooltip("What layer is the ground")]
        public LayerMask groundLayer;

        public Action<bool> OnGroundedChanged;
        private bool grounded = false;
        private RaycastHit hit;


        private Vector3 direction;
        float distance;


        /// <summary>
        /// The most recent hit
        /// </summary>
        public RaycastHit Hit { get => hit; }
        public bool Grounded
        {
            get => grounded;
            private set
            {
                if (grounded != value)
                {
                    grounded = value;
                    OnGroundedChanged?.Invoke(value);
                }
            }

        }
        private void OnEnable()
        {
            direction = startCast.position.To(endCast.position);
            distance = direction.magnitude;

            FixedUpdateEvent.Initialize();
            FixedUpdateEvent.OnFixedUpdate += GroundDetect;
        }
        private void OnDisable()
        {

            FixedUpdateEvent.OnFixedUpdate -= GroundDetect;
        }
        private void GroundDetect()
        {
            Grounded = Physics.Raycast(startCast.position, direction, out hit, distance, groundLayer);
        }
    }
}