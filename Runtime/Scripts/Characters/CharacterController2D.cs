using UnityEngine;
using AugustEngine.Input;
using AugustEngine.LowLevel;
using AugustEngine.Physics;
using System;
using AugustEngine.Physics.Gravity;

namespace AugustEngine.Characters
{
    /// <summary>
    /// A basic 2D character controller meant to be inherited and overridden. Takes care of default input registering.
    /// </summary>
    public class CharacterController2D
        : MonoBehaviour
    {
        [SerializeField] private InputChannel inputChannel;
        [SerializeField] private TargetVelocitySetter targetVelocitySetter;
        [SerializeField] private GroundDetector groundDetector;
        [SerializeField] CharacterMovementSettings movementSettings;


        [Serializable]
        internal class CharacterMovementSettings
        {
            public float maxSpeed;
            public float groundedAcceleartionForce;
            public float inAirAcceleartionForce;
            public float maxJump;
        }

        #region Input Events
        private Vector2 _moveStick = new();
        private void HandleMoveStick(object val) => _moveStick = (Vector2)val;
        private void HandleButton1(object jumpDown)
        {
            if ((float)jumpDown > 0.5f)
            {
                Jump();
            }
        }
        private void HandleButton2(object value)
        {
            if ((float)value > 0.5f)
            {
                Button2();
            }

        }
        private void HandleButton3(object value)
        {
            if ((float)value > 0.5f)
            {
                Button3();
            }
        }
        #endregion

        #region Event Registration
        private void RegisterAll()
        {
            inputChannel.RegisterCallback("Move", HandleMoveStick);
            inputChannel.RegisterCallback("Button1", HandleButton1);
            inputChannel.RegisterCallback("Button2", HandleButton2);
            inputChannel.RegisterCallback("Button3", HandleButton3);
            FixedUpdateEvent.OnFixedUpdate += FixedUpdateLoop;
        }

        private void DeregisterAll()
        {
            inputChannel.UnregisterCallback("Move", HandleMoveStick);
            inputChannel.UnregisterCallback("Button1", HandleButton1);
            inputChannel.UnregisterCallback("Button2", HandleButton2);
            inputChannel.UnregisterCallback("Button3", HandleButton3);
            FixedUpdateEvent.OnFixedUpdate -= FixedUpdateLoop;
        }
        #endregion

        #region Unity Events
        protected virtual void Awake()
        {
            FixedUpdateEvent.Initialize();
        }
        protected virtual void OnEnable()
        {
            RegisterAll();
        }
        protected virtual void OnDisable()
        {
            DeregisterAll();
        }
        #endregion


        protected virtual void FixedUpdateLoop()
        {
            OnMove(_moveStick);
        }



        #region Movement Actions
        protected virtual void OnMove(Vector2 moveStick)
        {
            // update acceleration
            targetVelocitySetter.Acceleration = groundDetector.Grounded ? movementSettings.groundedAcceleartionForce : movementSettings.inAirAcceleartionForce;

            targetVelocitySetter.TargetVelocity = moveStick * movementSettings.maxSpeed;

        }
        protected virtual void Jump()
        {
            if (groundDetector.Grounded)
            {
                targetVelocitySetter.RB.velocity = new Vector3(
                    targetVelocitySetter.RB.velocity.x,
                    movementSettings.maxJump,
                    targetVelocitySetter.RB.velocity.z

                    );
            }
        }


        protected virtual void Button1() { }

        protected void Button2() { }

        protected void Button3() { }
        #endregion

    }
}
