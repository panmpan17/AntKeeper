using System;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MovementAxis { get; private set; }
    public event Action OnMovementPerformedEvent;

    public event Action OnInteractPerformedEvent;
    public event Action OnInteractCanceledEvent;

    public event Action OnDashPerformedEvent;

    private InputScheme _inputScheme;

    void Awake()
    {
        _inputScheme = new InputScheme();

        _inputScheme.Game.Movement.performed += HandleMovementPerformed;
        _inputScheme.Game.Movement.canceled += HandleMovementPerformed;

        _inputScheme.Game.Interact.performed += HandleInteractPerformed;
        _inputScheme.Game.Interact.canceled += HandleInteractCanceled;

        _inputScheme.Game.Dash.performed += HandleDashPerformed;
    }

    void HandleMovementPerformed(CallbackContext callbackContext)
    {
        MovementAxis = callbackContext.ReadValue<Vector2>();
        OnMovementPerformedEvent?.Invoke();
    }

    void HandleInteractPerformed(CallbackContext callbackContext)
    {
        OnInteractPerformedEvent?.Invoke();
    }

    void HandleInteractCanceled(CallbackContext callbackContext)
    {
        OnInteractCanceledEvent?.Invoke();
    }

    void HandleDashPerformed(CallbackContext callbackContext)
    {
        OnDashPerformedEvent?.Invoke();
    }

    void OnEnable()
    {
        _inputScheme.Enable();
    }
    void OnDisable()
    {
        _inputScheme.Disable();
    }
}
