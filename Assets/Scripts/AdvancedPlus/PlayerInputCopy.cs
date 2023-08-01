using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCopy : MonoBehaviour
{
    #region INITIALIZE INPUT
    
    //private InputActions _inputActions;
    private InputTest _inputActions;
    private void Awake() => _inputActions = new InputTest();
    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    #endregion

    public Vector2 Move() { return _inputActions.Player.Move.ReadValue<Vector2>(); }
        // How to Use: if (Move().x != 0) { do stuff } ;
    public Vector2 Look() { return _inputActions.Player.Look.ReadValue<Vector2>(); }
    public InputAction Jump() { return _inputActions.Player.Jump; }
        // How to Use:  
        // if (Jump().triggered || Jump().IsPressed || Jump().WasReleasedThisFrame()) { do stuff }
    public InputAction Interact() { return _inputActions.Player.Interact; }
}