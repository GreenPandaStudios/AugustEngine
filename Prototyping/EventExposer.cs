namespace AugustEngine.Prototyping
{


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Exposes events for unityactions
    /// </summary>
    public class EventExposer : MonoBehaviour
    {
        public void Destroy() => Destroy(this);

    }
}
