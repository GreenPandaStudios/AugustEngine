namespace AugustEngine.LowLevel
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    /// <summary>
    /// Pritects a type of T and sends an event when it is changed
    /// </summary>
    public class ChangeListener<T>
    {
        public Action<T> OnChanged = delegate { };
        private T localvalue;
        public T Value
        {
            get => localvalue;
            set
            {
                if (!value.Equals(localvalue))
                {
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
    }
}