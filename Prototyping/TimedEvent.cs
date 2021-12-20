namespace AugustEngine.Prototyping
{





    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Sends out an object after a certian amount of time passes
    /// </summary>
    public class TimedEvent : MonoBehaviour
    {
        [SerializeField] UnityEvent onTimerEnd;
        [SerializeField] float time;
        private void OnEnable()
        {
            StartCoroutine(DelayedInvoke());

        }

        IEnumerator DelayedInvoke()
        {
            yield return new WaitForSecondsRealtime(time);
            onTimerEnd?.Invoke();
        }
    }
}