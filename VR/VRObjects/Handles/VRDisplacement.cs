namespace AugustEngine.VR.VRObjects.Handles
{
    /// Exports the displacement vector as an event
    /// from the intitial position of a xr controller



    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.XR.Interaction.Toolkit;
    using Vectors;
    public class VRDisplacement : MonoBehaviour
    {
        [SerializeField] [Tooltip("What controller interacts with this?")] ActionBasedController controller;
        [SerializeField] [Tooltip("The vector should be not larger than")] float maxMagnitude = 1f;
        [SerializeField] [Tooltip("Multiply the real-world displacement by")] float multiplier = 1f;
        [SerializeField] [Tooltip("In the space of what")] Transform localSpace;
        [SerializeField] [Tooltip("What transform to use to calcualte the offset")] Transform offsetOrigin;
        public Action<Vector3> OnDisplacementChange;

        private Vector3 displacment;
        public Vector3 Displacemnt
        {
            get => displacment;
            private set
            {
                if (displacment != value)
                {
                    displacment = value;
                    OnDisplacementChange?.Invoke(value);
                }
            }
        }
        private Vector3 ControllerPos { get => controller.transform.position; }
        public void Grab()
        {
            //move the offset to the place we started at
            offsetOrigin.position = ControllerPos;
            controller.SendHapticImpulse(.5f, .25f);
            LowLevel.UpdateEvent.OnUpdate += UpdateDisplacement;
        }
        public void Drop()
        {

            Displacemnt = Vector3.zero;
            controller.SendHapticImpulse(.15f, .15f);
            LowLevel.UpdateEvent.OnUpdate -= UpdateDisplacement;
        }
        private void OnEnable()
        {
            //ensure the hook is activated
            if (LowLevel.UpdateEvent.Instance) { }
        }
        private void UpdateDisplacement()
        {
            //calcuate the displacement in local space
            Displacemnt = (offsetOrigin.position.To(ControllerPos) * multiplier)
                .ToLocalSpace(localSpace);
        }
    }

}

