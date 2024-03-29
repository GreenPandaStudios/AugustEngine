namespace AugustEngine.LowLevel
{
    using System;


    public class FixedUpdateEvent : Singleton<FixedUpdateEvent>
    {
        /// <summary>
        /// Event thrown on the unity engine's fixed update call
        /// </summary>
        public static Action OnFixedUpdate;

        // Update is called once per frame
        void FixedUpdate() => OnFixedUpdate?.Invoke();
    }
}