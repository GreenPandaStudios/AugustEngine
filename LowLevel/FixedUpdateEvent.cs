namespace AugustEngine.LowLevel
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;


    public class FixedUpdateEvent : Singleton<UpdateEvent>
    {

        public static Action OnFixedUpdate;

        // Update is called once per frame
        void FixedUpdate() => OnFixedUpdate?.Invoke();
    }
}