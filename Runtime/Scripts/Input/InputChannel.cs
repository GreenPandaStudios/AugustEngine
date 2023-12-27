using System.Collections.Generic;
using Unity.VisualScripting;
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
        private Dictionary<string, System.Action<object>> _registrationQueue;


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

            // Clear registration queue if it is not empty
            if (_registrationQueue != null && _registrationQueue.Count > 0) {
                foreach (string action in _registrationQueue.Keys)
                {
                    RegisterCallback(action, _registrationQueue[action]);
                }
                _registrationQueue = new();
            }
        }
        public void RegisterCallback(string actionName, System.Action<object> callBack)
        {
            if (inputEvents == null)
            {
                QueueEvent(actionName, callBack);
                return;
            }


            if (inputEvents.ContainsKey(actionName))
            {
                inputEvents[actionName] += callBack;
                return;
            }
            Debug.LogError($"Tried to register an nonexistant action, {actionName}");
        }


        private void QueueEvent(string actionName, System.Action<object> callBack)
        {
            if (_registrationQueue == null)
            {
                _registrationQueue = new();
            }
            _registrationQueue.Add(actionName, callBack);
        }

        public void UnregisterCallback(string actionName, System.Action<object> callBack)
        {
            if (inputEvents == null) { return; }

            if (inputEvents.ContainsKey(actionName))
            {
                inputEvents[actionName] -= callBack;
                return;
            }



            string _actions = "[";
            foreach (string key in inputEvents.Keys)
            {
                _actions += key + ", ";
            }
            _actions += "]";


            Debug.LogError($"Tried to unregister an nonexistant action, {actionName}. List of existing actions: {_actions}");
        }

    }
}