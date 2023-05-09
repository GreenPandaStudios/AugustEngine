using System.Collections.Generic;
using UnityEngine;
namespace AugustEngine.Input
{
    /// <summary>
    /// A Channel to broadcast input events. See <see cref="InputBroadcaster"/> to broadcast these events in a scene
    /// </summary>
    [CreateAssetMenu(fileName = "Input Channel 1", menuName = "August Engine/Input/Input Channel", order = 1)]
    public class InputChannel : ScriptableObject
    {
        private Dictionary<string, System.Action<object>> inputEvents;
        public void ConstructEvents(UnityEngine.InputSystem.InputActionMap inputActions)
        {
            if (inputEvents !=null && inputEvents.Count != 0)
            {
                foreach (var action in inputActions)
                {
                    action.performed -= obj => inputEvents[action.name](obj.action.ReadValueAsObject());
                }
            }

            inputEvents = new Dictionary<string, System.Action<object>>();
            foreach (var action in inputActions)
            {
                inputEvents[action.name] = delegate { };
                action.performed += obj => inputEvents[action.name](obj.action.ReadValueAsObject());
            }
        }
        public void RegisterCallback(string actionName, System.Action<object> callBack)
        {
            if (inputEvents.ContainsKey(actionName))
            {
                inputEvents[actionName] += callBack;
                return;
            }
            Debug.LogError($"Tried to register an nonexistant action, {actionName}");
        }
        public void UnregisterCallback(string actionName, System.Action<object> callBack)
        {
            if (inputEvents.ContainsKey(actionName))
            {
                inputEvents[actionName] -= callBack;
                return;
            }
            Debug.LogError($"Tried to unregister an nonexistant action, {actionName}");
        }

    }
}