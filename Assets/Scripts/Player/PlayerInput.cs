using System;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInputEvent inputEvent;

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

        inputEvent.OnMovementAxisChange += OnMobileMovementPerformed;
        inputEvent.OnDash += OnMobileDashButtonPerformed;
        inputEvent.OnInteractPerformed += OnMobileInteractPerforemed;
        inputEvent.OnInteractCanceled += OnMobileInteractCanceled;
    }


    #region Input Action handle
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
    #endregion


    #region Mobile UI button handle
    public void OnMobileMovementPerformed(Vector2 axis)
    {
        if (!enabled) return;
        MovementAxis = axis;
        OnMovementPerformedEvent?.Invoke();
    }

    public void OnMobileDashButtonPerformed()
    {
        if (!enabled) return;
        OnDashPerformedEvent?.Invoke();
    }

    public void OnMobileInteractPerforemed()
    {
        if (!enabled) return;
        OnInteractPerformedEvent?.Invoke();
    }

    public void OnMobileInteractCanceled()
    {
        if (!enabled) return;
        OnInteractCanceledEvent?.Invoke();
    }
    #endregion


    void OnEnable()
    {
        _inputScheme.Enable();
    }
    void OnDisable()
    {
        _inputScheme.Disable();
    }
}
