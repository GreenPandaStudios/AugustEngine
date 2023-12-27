namespace AugustEngine.CameraRig
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.Vectors;
    public class SimpleFollowRig : MonoBehaviour
    {
        [SerializeField] Transform lookAtTarget;
        [SerializeField] Transform followTarget;
        [SerializeField] [Range(0f, 1f)] float followStrength;
        [SerializeField] [Range(0f, 1f)] float lookStrength;

        private void OnEnable()
        {
            LowLevel.FixedUpdateEvent.Initialize();
            LowLevel.FixedUpdateEvent.OnFixedUpdate += MoveRig;
        }
        private void OnDisable()
        {
            LowLevel.FixedUpdateEvent.OnFixedUpdate -= MoveRig;
        }
        // Update is called once per frame
        void MoveRig()
        {
            transform.position = Vector3.Lerp(transform.position, followTarget.position, followStrength);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(transform.position.To(lookAtTarget.position), Vector3.up), lookStrength);
        }
    }
}