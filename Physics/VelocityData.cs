namespace AugustEngine.Physics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    public class VelocityData : MonoBehaviour
    {

        private Vector3 lastPos;
        private Vector3 localLastPos;
        private Vector3 velocity;
        private Vector3 localVelocity;
        public Vector3 Velocity { get => velocity; }
        public Vector3 LocalVelocity { get => localVelocity; }

        public Vector3 Position { get => transform.position; }
        private void OnEnable()
        {
            lastPos = transform.position;
            localLastPos = transform.localPosition;
            FixedUpdateEvent.Initialize();
            FixedUpdateEvent.OnFixedUpdate += RecalVeclocity;
        }
        private void OnDisable()
        {
            FixedUpdateEvent.OnFixedUpdate -= RecalVeclocity;
        }


        //recalculate velocity{
        public void RecalVeclocity()
        {
            localVelocity = (transform.localPosition - localLastPos) / Time.fixedDeltaTime;
            localLastPos = transform.localPosition;


            velocity = (transform.position - lastPos) / Time.fixedDeltaTime;
            lastPos = transform.position;
        }
    }
}


