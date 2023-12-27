namespace AugustEngine.Physics
{

    using System;
    using UnityEngine;
    using AugustEngine.Vectors;
    /// <summary>
    /// Used to streamline the process of checking if a character is grounded
    /// </summary>
    [System.Serializable]
    public class GroundChecker
    {
        private LayerMask groundMask;
        private Transform fromTransform;
        private Transform toTransform;

        /// <summary>
        /// GroundChecker constructor
        /// </summary>
        /// <param name="groundMask">The layermask specifying what is the "ground"</param>
        /// <param name="fromTransform">The origin point where the ray will be drawn</param>
        /// <param name="toTransform">The ending point where the ray will be drawn to</param>
        public GroundChecker(LayerMask groundMask, Transform fromTransform, Transform toTransform)
        {
            this.groundMask = groundMask;
            this.fromTransform = fromTransform;
            this.toTransform = toTransform;
        }
        private int curCooldown = 0;
        /// <summary>
        /// Wait the specified number of updates to wait before switching state again
        /// </summary>
        public void Cooldown(int waitFrames)
        {
            curCooldown = waitFrames;
        }
        public Action<bool> OnGroundedChanged;
        public Action<RaycastHit> OnHitChange;

        private bool grounded;
        /// <summary>
        /// This is true when the player is on the ground
        /// </summary>
        public bool Grounded { get => grounded; set
            {
                grounded = value;
                OnGroundedChanged?.Invoke(grounded);
            }
        }
        private RaycastHit hitInfo;
        /// <summary>
        /// This is the last hit info the player was standing on
        /// </summary>
        public RaycastHit HitInfo { get => hitInfo; }

        /// <summary>
        /// Calling this will update the <see cref="grouned"/> variable
        /// </summary>
        public void UpdateGroundCheck()
        {
            if (curCooldown > 0)
            {
                curCooldown--;
                return;
            }


            var _vec = fromTransform.position.To(toTransform.position);

            //raycast the target
            var _result = Physics.Raycast(
                fromTransform.position,
                _vec,
                 out var _hit,
                _vec.magnitude,
                groundMask,
                QueryTriggerInteraction.UseGlobal
                );

            if (_result)
            {
                hitInfo = _hit;
                OnHitChange?.Invoke(hitInfo);
            }

            if (_result != grounded)
            {
                Grounded = _result;             
            }

        }


      
    }
}