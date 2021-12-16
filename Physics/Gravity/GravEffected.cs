namespace AugustEngine.Phyics.Gravity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GravEffected : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The rigidbody to apply the force to")]
        Rigidbody rb;

        public Rigidbody RB { get => rb; }
    }
}