namespace AugustEngine.Procedural
{
    using AugustEngine.Vectors;
    using AugustEngine.LowLevel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Used for an infinite world to get rif of floating point error
    /// Will shift objects to stay around the origin
    /// </summary>
    public class InfiniteWorldTransform : MonoBehaviour
    {



        protected static ChangeListener<Vector3Double> globalOffset = new ChangeListener<Vector3Double>(Vector3Double.Zero);
        protected static Action<Vector3Double> shiftByAmount;
        public static Vector3Double GlobalOffset
        {
            get => globalOffset.Value;
        }
        public static Action<Vector3Double> GlobalOffsetChange
        {
            get => globalOffset.OnChanged;
            set => globalOffset.OnChanged = value;
        }
        public Vector3Double position {
            get
            {
                return GlobalOffset + (Vector3Double)transform.position;
            }
            set
            {
                var _position =
                    value - GlobalOffset;

                transform.position = new Vector3((float)_position.x, (float)_position.y, (float)_position.z);
            }
        }
        private void OnEnable()
        {
            Shift(Vector3.zero);
            shiftByAmount += Shift;
           
        }
        private void OnDisable()
        {
            shiftByAmount -= Shift;
        }

        private void Shift(Vector3Double shift)
        {
            position = shift;
        }
    }
}