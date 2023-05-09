namespace AugustEngine.LowLevel
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    /// <summary>
    /// Protects access to a type of T and sends an event when it is changed
    /// </summary>
    public class ChangeListener<T>
    {
        public Action<T> OnChanged = delegate { };
        private T localvalue;

        /// <summary>
        /// An event that will validate the change
        /// </summary>
        /// <param name="prevVal">The value we current have</param>
        /// <param name="newVal">The value we are changing to</param>
        /// <returns>True if the change should occur, false otherwise</returns>

        public delegate bool validateChange(T prevVal, T newVal);
        
        validateChange validateCallback;
        public T Value
        {
            get => localvalue;
            set
            {
                if (!value.Equals(localvalue))
                {
                    if (validateCallback!= null)
                    {

                        if (!validateCallback(localvalue, value))
                        {
                            // Do not change the value
                            return;
                        }
                    }


                    localvalue = value;
                    OnChanged?.Invoke(localvalue);
                }
            }
        }

        public ChangeListener(T defaultValue)
        {
            localvalue = defaultValue;
            OnChanged?.Invoke(localvalue);
        }

        /// <summary>
        /// Creates a chance listener
        /// </summary>
        /// <param name="defaultValue">What to a initialize this value with</param>
        /// <param name="validateCallback">A callback to validate the change. Return false to stop the change from occuring</param>
        public ChangeListener(T defaultValue, validateChange validateCallback)
        {
            localvalue = defaultValue;
            this.validateCallback = validateCallback;
            OnChanged?.Invoke(localvalue);
        }
    }
}