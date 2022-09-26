using UnityEngine;
using UnityEngine.InputSystem;

namespace _Beginner
{
    public class PlayerInput : MonoBehaviour
    {
        #region INITIALIZE INPUT
        
        //private InputActions _inputActions;
        private InputTest _inputActions;
        private void Awake() => _inputActions = new InputTest();
        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();

        #endregion

        #region INPUT VARIABLES
        
        public Vector2 MoveDirection { get; private set; }
        public Vector2 LookDirection { get; private set; }
        
        public bool JumpPressed { get; private set; }
        public bool InteractPressed { get; private set; }

        #endregion
        
        private void Update()
        {
            MoveDirection = _inputActions.Player.Move.ReadValue<Vector2>();
            LookDirection = _inputActions.Player.Look.ReadValue<Vector2>();

            JumpPressed = _inputActions.Player.Jump.triggered;
            InteractPressed = _inputActions.Player.Interact.triggered;
        }
    }
}