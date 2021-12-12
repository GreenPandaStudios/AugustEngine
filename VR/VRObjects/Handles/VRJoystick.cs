namespace AugustEngine.VR.VRObjects.Handles
{


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.XR.Interaction.Toolkit;
    using AugustEngine.LowLevel;


    /// <summary>
    /// A virtual joystick allowing for directional input
    /// </summary>
    public class VRJoystick : XRGrabInteractable
    {
        [Header("Transforms")]
        [SerializeField]
        [Tooltip("The mesh representing the joystick")]
        Transform joyMeshSpace;
        [SerializeField]
        [Tooltip("The transform representing the joystick's local space")]
        Transform joyStickSpace;
        [SerializeField]
        [Tooltip("The origin point that this returns to when released")]
        Transform originPoint;
        [Header("Numeric Values")]
        [SerializeField]
        [Range(0f, 1f)]
        float deadZone = .1f
            ;
        [SerializeField]
        [Range(0f, 10f)]
        float dropDistance = 2f
            ;
        [SerializeField]
        bool noX;
        [SerializeField]
        bool noY;

        private void Start()
        {
            Debug.Log(UpdateEvent.Instance);
        }
        ///<summary>An input event thrown when the  joystick's
        ///virtual input changes.</summary>
        public Action<Vector2> OnJoyInputChange;

        private Vector2 joyInput = new Vector2();
        public Vector2 JoyInput
        {
            get => joyInput;
            private set => SetJoyInput(value);
        }

        /// <summary>
        /// A setter for the joyInput
        /// </summary>
        /// <param name="joyInput"></param>
        private void SetJoyInput(Vector2 value)
        {
            //don't do anything unless the input actually changed
            if (value == joyInput) return;
            if (value.sqrMagnitude < (deadZone * deadZone))
            {
                joyInput = Vector2.zero;
            }
            else
            {
                joyInput = value;
            }

            OnJoyInputChange?.Invoke(value);
        }


        protected override void Grab()
        {
            UpdateEvent.OnUpdate += UpdateMesh;
        }

        protected void Drop(bool updateMesh)
        {
            UpdateEvent.OnUpdate -= UpdateMesh;
            transform.position = originPoint.position;
            //update the mesh the final time
            if (updateMesh) UpdateMesh();
        }
        protected override void Drop() => Drop(true);
        protected void UpdateMesh()
        {
            //total offset of target from handle
            var _offset = (transform.position - joyMeshSpace.position);

            //too far, drop? (don't update mesh)
            if (_offset.sqrMagnitude > dropDistance * dropDistance) Drop(false);

            //calculate the local-space input
            var local_space_input = new Vector2();

            local_space_input.x = noX ? 0f : Vector3.Dot(joyStickSpace.right, _offset);
            local_space_input.y = noY ? 0f : Vector3.Dot(joyStickSpace.forward, _offset);

            //make the mesh update
            joyMeshSpace.transform.up = joyStickSpace.up +
                joyStickSpace.right * local_space_input.x * 3f
                +
                joyStickSpace.forward * local_space_input.y * 3f
                ;

            JoyInput = local_space_input;
        }
    }
}