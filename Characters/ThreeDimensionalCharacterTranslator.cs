using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AugustEngine.Physics;
using AugustEngine.Vectors;
using AugustEngine.LowLevel;
namespace AugustEngine.Characters
{
    /// <summary>
    /// Meant to handle translations of physics-based 3D characters
    /// Will register/Init ground, acceleration changes and fixed update hook, but does not
    /// register any input handling
    /// </summary>
    [RequireComponent(typeof(TargetVelocitySetter))]
    public abstract class ThreeDimensionalCharacterTranslator : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] protected TargetVelocitySetter velSetter;
        protected GroundChecker groundChecker;
        [SerializeField] protected Collider playerCollider;
        [Tooltip("Velocity Change will set the jump to a predictable velocity each time")]
        [SerializeField] protected ForceMode jumpForceMode = ForceMode.VelocityChange;
        [Header("Transforms")]
        [SerializeField] protected Transform cameraSpace;
        [SerializeField] protected Transform groundCheckFrom;
        [SerializeField] protected Transform groundCheckTo;

        [Header("Constants")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] protected float jumpPower;
        [SerializeField] protected float speed;
        [SerializeField] protected float inAirAccel;
        [SerializeField] protected float groundedAccel;

        #region Convienent references


        private enum JumpMode
        {
            Addition,

        }


        public virtual Rigidbody RB { get => velSetter.RB; }
        public virtual bool Grounded { get => groundChecker.Grounded; }
        public virtual float Acceleration { get => velSetter.Acceleration; protected set => velSetter.Acceleration = value; }
       
        public virtual Vector3 ForwardAccoringToCam { get => transform.forward.ToCameraSpace(cameraSpace); }
        public virtual Vector3 RightAccoringToCam { get => transform.right.ToCameraSpace(cameraSpace); }

        #endregion


        private float normalFric;
        private PhysicMaterial instancePhysicsMat;
        private PhysicMaterial startingPhysicsMat;

        protected virtual void UpdateAccel(bool grounded)
        {
            Acceleration = grounded ? groundedAccel : inAirAccel;
            instancePhysicsMat.dynamicFriction = grounded ? startingPhysicsMat.dynamicFriction : 0;
            instancePhysicsMat.staticFriction = grounded ? startingPhysicsMat.staticFriction : 0;
        }

        public virtual void Initialize()
        {
            Teardown();

            groundChecker = new GroundChecker(whatIsGround,groundCheckFrom, groundCheckTo);



            startingPhysicsMat = instancePhysicsMat = playerCollider.sharedMaterial;
            playerCollider.material = instancePhysicsMat;

            groundChecker.OnGroundedChanged += UpdateAccel;
            UpdateAccel(Grounded);


            FixedUpdateEvent.Initialize();
            FixedUpdateEvent.OnFixedUpdate += FixedUpdateHook;

        }
        public virtual void Teardown()
        {
            FixedUpdateEvent.OnFixedUpdate -= FixedUpdateHook;
            if (groundChecker != null)
            {
                groundChecker.OnGroundedChanged = delegate { };
                groundChecker = null;
            }

            if (instancePhysicsMat)
            {
                playerCollider.sharedMaterial = startingPhysicsMat;
            }
           
        }



        public virtual void Jump()
        {
            if (Grounded)
            {
                groundChecker.Grounded = false;
                groundChecker.Cooldown(5);
                switch (jumpForceMode)
                {
                    case ForceMode.VelocityChange:
                        RB.velocity = new Vector3(RB.velocity.x, jumpPower, RB.velocity.z);
                        break;
                    default:
                        RB.AddForce(Vector3.up * jumpPower, jumpForceMode);
                        break;
                }
                
            }
        }

        public virtual void Move(Vector2 input)
        {
            velSetter.TargetVelocity = input.ToXZPlane().ToCameraSpace(cameraSpace);
        }


        public abstract void FixedUpdateHook();
    }

}

