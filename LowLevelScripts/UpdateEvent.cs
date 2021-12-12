namespace AugustEngine.LowLevel
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;


    public class UpdateEvent : Singleton<UpdateEvent>
    {

        public static Action OnUpdate;
        // Update is called once per frame
        void Update() => OnUpdate?.Invoke();
    }
}


