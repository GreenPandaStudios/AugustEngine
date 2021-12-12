namespace AugustEngine.Phyics
{



    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    public class HoverPoint : MonoBehaviour
    {
        [SerializeField] LayerMask whatIsHoverable;
        [SerializeField] Transform hoverPoint;
        [SerializeField] float strength;
        [SerializeField] float maxDist;
        [SerializeField] [Range(0f, 1f)] float dampening;
        [SerializeField] Collider addForceToWhat;
        private float last_pow = 100f;
        private void OnEnable()
        {
            //make sure fixedupdate is instantiated
            if (FixedUpdateEvent.Instance) { }
            FixedUpdateEvent.OnFixedUpdate += Hover;

        }
        private void OnDisable()
        {
            FixedUpdateEvent.OnFixedUpdate -= Hover;
        }
        // Update is called once per frame
        void Hover()
        {
            var _boardPoint = addForceToWhat.ClosestPointOnBounds(hoverPoint.position);
            var _pow = maxDist;
            if (Physics.Raycast(_boardPoint, -addForceToWhat.transform.up, out var _hit, maxDist, whatIsHoverable))
            {
                //get the distance from the board point to the hit point
                _pow = Mathf.Max(Vector3.SqrMagnitude(_hit.point - _boardPoint), .015f);

            }
            //apply an inverse force to the board
            addForceToWhat.attachedRigidbody.AddForceAtPosition(addForceToWhat.transform.up * (strength / Mathf.Lerp(_pow, last_pow, dampening)), _boardPoint, ForceMode.Force);

            last_pow = Mathf.Max(_pow, last_pow);
        }
    }
}