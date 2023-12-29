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
        [SerializeField] protected InputChannel inputChannel;
        [SerializeField] protected TargetVelocitySetter targetVelocitySetter;
        [SerializeField] protected GroundDetector groundDetector;
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
        private void HandleButton1(object jumpDown) => Jump((float)jumpDown > 0.5f);
        private void HandleButton2(object value) => Button2((float)value > 0.5f);
        private void HandleButton3(object value) => Button3((float)value > 0.5f);
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
        protected virtual void Jump(bool buttonDown)
        {
            if (buttonDown && groundDetector.Grounded)
            {
                targetVelocitySetter.RB.velocity = new Vector3(
                    targetVelocitySetter.RB.velocity.x,
                    movementSettings.maxJump,
                    targetVelocitySetter.RB.velocity.z

                    );
                if (groundDetector.Hit.rigidbody != null )
                {
                    groundDetector.Hit.rigidbody.AddForceAtPosition(movementSettings.maxJump * Vector3.down * targetVelocitySetter.RB.mass,groundDetector.Hit.point,ForceMode.Impulse);
                }
                
            }
        }


        protected virtual void Button1(bool buttonDown) { }

        protected void Button2(bool buttonDown) { }

        protected void Button3(bool buttonDown) { }
        #endregion

    }
}
