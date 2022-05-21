
namespace AugustEngine.Input
{


    using UnityEngine.InputSystem;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AugustEngine.LowLevel;
    public class InputManager : MonoBehaviour
    {
        public ChangeListener<Vector2> MoveVector = new ChangeListener<Vector2>(Vector2.zero);
        public ChangeListener<Vector2> LookVector = new ChangeListener<Vector2>(Vector2.zero);
        public ChangeListener<float> Button1 = new ChangeListener<float>(0f);
        public ChangeListener<float> Button2 = new ChangeListener<float>(0f);
        public ChangeListener<float> Button3 = new ChangeListener<float>(0f);
        private DefaultInput inputActions;
        private void OnEnable()
        {
            inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.InGame.Move.performed += obj => MoveVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Look.performed += obj => LookVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Button1.performed += obj => Button1.Value = obj.ReadValue<float>();
            inputActions.InGame.Button2.performed += obj => Button2.Value = obj.ReadValue<float>();
            inputActions.InGame.Button3.performed += obj => Button3.Value = obj.ReadValue<float>();

        }
        private void OnDisable()
        {
            
            inputActions.InGame.Move.performed -= obj => MoveVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Look.performed -= obj => LookVector.Value = obj.ReadValue<Vector2>();
            inputActions.InGame.Button1.performed -= obj => Button1.Value = obj.ReadValue<float>();
            inputActions.InGame.Button2.performed -= obj => Button2.Value = obj.ReadValue<float>();
            inputActions.InGame.Button3.performed -= obj => Button3.Value = obj.ReadValue<float>();
            inputActions = null;
        }

    }
}