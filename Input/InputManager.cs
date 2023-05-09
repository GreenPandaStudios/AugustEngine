namespace AugustEngine.Input
{


    using UnityEngine.InputSystem;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    [System.Obsolete("Use the more extensible InputChannel")]
    public class InputManager : MonoBehaviour
    {
        public ChangeListener<Vector2> MoveVector = new ChangeListener<Vector2>(Vector2.zero);
        public ChangeListener<Vector2> LookVector = new ChangeListener<Vector2>(Vector2.zero);
        public ChangeListener<float> Button1 = new ChangeListener<float>(0f);
        public ChangeListener<float> Button2 = new ChangeListener<float>(0f);
        public ChangeListener<float> Button3 = new ChangeListener<float>(0f);
        public ChangeListener<float> Button4 = new ChangeListener<float>(0f);
        private DefaultInput inputActions;
        private void OnEnable()
        {
            inputActions = new DefaultInput();
            ActiveInputs.Add(this);
            if (!MapInputToDevice())
            {
                Debug.LogError("No device available for the input");
                OnDisable();
                return;
            }
            inputActions.Enable();
            inputActions.InGame.Move.performed += obj => MoveVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Look.performed += obj => LookVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Button1.performed += obj => Button1.Value = obj.ReadValue<float>();
            inputActions.InGame.Button2.performed += obj => Button2.Value = obj.ReadValue<float>();
            inputActions.InGame.Button3.performed += obj => Button3.Value = obj.ReadValue<float>();
            inputActions.InGame.Button4.performed += obj => Button4.Value = obj.ReadValue<float>();

        }
        private void OnDisable()
        {

            inputActions.InGame.Move.performed -= obj => MoveVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Look.performed -= obj => LookVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Button1.performed -= obj => Button1.Value = obj.ReadValue<float>();
            inputActions.InGame.Button2.performed -= obj => Button2.Value = obj.ReadValue<float>();
            inputActions.InGame.Button3.performed -= obj => Button3.Value = obj.ReadValue<float>();
            inputActions.InGame.Button4.performed -= obj => Button4.Value = obj.ReadValue<float>();

            foreach (InputDevice inputDevice in inputActions.devices)
            {
                inputDevices.Remove(inputDevice);
            }
            ActiveInputs.Remove(this);
            inputActions = null;
        }

        private static bool DeviceIsMapped(InputDevice device) { return inputDevices.Contains(device); }
        private bool MapInputToDevice()
        {
            List<InputDevice> devices = new List<InputDevice>();
            foreach (InputDevice device in InputSystem.devices)
            {

                if (!DeviceIsMapped(device))
                {
                    if (device is Gamepad && devices.Count == 0)
                    {
                        //only need one gamepad
                        devices.Add(device);
                        inputDevices.Add(device);
                        break;

                    }
                    else if (device is not Gamepad)
                    {
                        //need all of anything else
                        devices.Add(device);
                        inputDevices.Add(device);
                    }
                }

            }
            if (devices.Count > 0)
            {
                inputActions.devices = new UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>(devices.ToArray());
                return true;
            }
            return false;
        }
        private static List<InputDevice> inputDevices = new List<InputDevice>();
        private static List<InputManager> ActiveInputs = new List<InputManager>();
     
        /// <summary>
        /// Returns the first mapped input
        /// </summary>
        public static InputManager DefaultInput
        {
            get
            {
                return ActiveInputs[0];
            }
        }
    }
}