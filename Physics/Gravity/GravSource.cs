namespace AugustEngine.Phyics.Gravity
{

    /// A source of gravity that will effect <seealso cref="GravEffected"/>

    using AugustEngine.LowLevel;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.Vectors;
    //The collider an object must be in to consider it as being affected by gravity
    [RequireComponent(typeof(SphereCollider))]
    public class GravSource : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The center of mass for this object")]
        Transform centerOfMass;
        [SerializeField]
        [Tooltip("The gravitational constant for this object")]
        float gravConst;


        LinkedList<GravEffected> effected = new LinkedList<GravEffected>();


        private void OnEnable()
        {
            //make sure the hook is enabled
            if (FixedUpdateEvent.Instance) { }
            if (effected.Count > 0)
            {
                FixedUpdateEvent.OnFixedUpdate += AddGravForce;
            }

        }
        private void OnDisable()
        {
            if (effected.Count > 0)
            {
                FixedUpdateEvent.OnFixedUpdate -= AddGravForce;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<GravEffected>(out var effected))
            {
                this.effected.AddFirst(effected);

                //if this is the first one, start adding the force
                if (this.effected.Count == 1)
                {
                    FixedUpdateEvent.OnFixedUpdate += AddGravForce;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<GravEffected>(out var effected))
            {
                this.effected.Remove(effected);

                //if this is the last one, stop adding the force
                if (this.effected.Count == 0)
                {
                    FixedUpdateEvent.OnFixedUpdate -= AddGravForce;
                }
            }
        }



        private void AddGravForce()
        {
            foreach (GravEffected gravEffected in effected)
            {
                var _dist = centerOfMass.position.From(gravEffected.RB.position);
                var _mag = _dist.sqrMagnitude;

                //don't allow a singularity
                if (_mag < .00125f) return;

                gravEffected.RB.AddForce(
                    _dist.normalized *
                    (gravConst / _mag)
                    );
            }
        }



    }
}


